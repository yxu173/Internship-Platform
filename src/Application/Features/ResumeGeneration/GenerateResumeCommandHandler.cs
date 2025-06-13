using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Domain.Aggregates.Resumes;
using Domain.Repositories;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Application.Features.ResumeGeneration;

public class GenerateResumeCommandHandler : ICommandHandler<GenerateResumeCommand, GenerateResumeResponse>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentProfileRepository;
    private readonly ICompanyRepository _companyProfileRepository;
    private readonly IGeneratedResumeRepository _resumeRepository;
    private readonly IGeminiAIService _geminiAIService;
    private readonly IPdfGenerationService _pdfGenerationService;
    private readonly IConfiguration _configuration;

    public GenerateResumeCommandHandler(
        IInternshipRepository internshipRepository,
        IStudentRepository studentProfileRepository,
        ICompanyRepository companyProfileRepository,
        IGeneratedResumeRepository resumeRepository,
        IGeminiAIService geminiAIService,
        IPdfGenerationService pdfGenerationService,
        IConfiguration configuration)
    {
        _internshipRepository = internshipRepository;
        _studentProfileRepository = studentProfileRepository;
        _companyProfileRepository = companyProfileRepository;
        _resumeRepository = resumeRepository;
        _geminiAIService = geminiAIService;
        _pdfGenerationService = pdfGenerationService;
        _configuration = configuration;
    }

    public async Task<Result<GenerateResumeResponse>> Handle(GenerateResumeCommand request, CancellationToken cancellationToken)
    {
        var studentProfile = await _studentProfileRepository.GetFullProfileByUserId(request.UserId);
        if (studentProfile == null)
            throw new InvalidOperationException($"Student with ID {request.UserId} not found");

        var internship = await _internshipRepository.GetByIdAsync(request.InternshipId);
        if (internship == null)
            throw new InvalidOperationException($"Internship with ID {request.InternshipId} not found");

        var companyProfile = await _companyProfileRepository.GetByCompanyIdAsync(internship.CompanyProfileId);
        if (companyProfile == null)
            throw new InvalidOperationException($"Company with ID {internship.CompanyProfileId} not found");

        var studentSkills = await _studentProfileRepository.GetStudentSkillsAsync(studentProfile.Id);
        var skills = studentSkills.Select(s => s.Skill).ToList();

        var resumeDto = ResumeDto.Create(
            studentProfile,
            internship,
            companyProfile,
            skills,
            studentProfile.Experiences,
            studentProfile.Projects);

        var existingResumes = await _resumeRepository.GetByStudentAndInternshipAsync(
            studentProfile.Id, request.InternshipId);

        foreach (var existingResume in existingResumes)
        {
            existingResume.MarkAsObsolete();
            await _resumeRepository.UpdateAsync(existingResume);
        }

        // Generate AI-powered resume content in markdown format
        var aiResult = await _geminiAIService.GenerateResumeMarkdownAsync(resumeDto, cancellationToken);
        if (aiResult.IsFailure)
            return Result.Failure<GenerateResumeResponse>(aiResult.Error);

        // Generate PDF from markdown content using IronPDF
        var pdfResult = await _pdfGenerationService.GenerateResumePdfAsync(aiResult.Value, cancellationToken);
        if (pdfResult.IsFailure)
            return Result.Failure<GenerateResumeResponse>(pdfResult.Error);

        // Create and save the generated resume record
        var generatedResumeResult = GeneratedResume.Create(
            studentProfile.Id,
            request.InternshipId,
            pdfResult.Value);

        if (generatedResumeResult.IsFailure)
            return Result.Failure<GenerateResumeResponse>(generatedResumeResult.Error);

        await _resumeRepository.AddAsync(generatedResumeResult.Value);

        // Generate download URL
        var baseUrl = _configuration["BaseUrl"] ?? "http://localhost:5000";
        var downloadUrl = $"{baseUrl}/api/resumes/download/{generatedResumeResult.Value.Id}";

        return Result.Success(new GenerateResumeResponse(
            generatedResumeResult.Value.Id,
            pdfResult.Value,
            downloadUrl));
    }
}