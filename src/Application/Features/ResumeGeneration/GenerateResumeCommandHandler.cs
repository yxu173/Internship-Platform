using Application.Abstractions.Messaging;
using Application.Abstractions.Services;
using Domain.Aggregates.Resumes;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;
using SharedKernel;

namespace Application.Features.ResumeGeneration;

public class GenerateResumeCommandHandler : ICommandHandler<GenerateResumeCommand, GenerateResumeResponse>
{
    private readonly IInternshipRepository _internshipRepository;
    private readonly IStudentRepository _studentProfileRepository;
    private readonly ICompanyRepository _companyProfileRepository;
    private readonly IGeneratedResumeRepository _resumeRepository;
    private readonly IGeminiClient _geminiClient;
    private readonly IPdfResumeRenderer _pdfRenderer;
    private readonly IConfiguration _configuration;
    private readonly ITemplateService _templateService;

    public GenerateResumeCommandHandler(
        IInternshipRepository internshipRepository,
        IStudentRepository studentProfileRepository,
        ICompanyRepository companyProfileRepository,
        IGeneratedResumeRepository resumeRepository,
        IGeminiClient geminiClient,
        IPdfResumeRenderer pdfRenderer,
        IConfiguration configuration,
        ITemplateService templateService)
    {
        _internshipRepository = internshipRepository;
        _studentProfileRepository = studentProfileRepository;
        _companyProfileRepository = companyProfileRepository;
        _resumeRepository = resumeRepository;
        _geminiClient = geminiClient;
        _pdfRenderer = pdfRenderer;
        _configuration = configuration;
        _templateService = templateService;
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

        byte[] templatePdf = null;
        if (!string.IsNullOrWhiteSpace(request.TemplateName))
        {
            templatePdf = await _templateService.GetResumeTemplateAsync(request.TemplateName, cancellationToken);
            if (templatePdf == null)
            {
                throw new InvalidOperationException($"Resume template '{request.TemplateName}' not found");
            }
        }
        
        var resumeContent = await _geminiClient.InvokeFunctionAsync<ResumeContent>(
            "generate_resume",
            resumeDto,
            templatePdf,
            cancellationToken);

        resumeContent.PhoneNumber = studentProfile.PhoneNumber.Value;
        resumeContent.Location = studentProfile.Location;

        var pdfBytes = _pdfRenderer.Render(resumeContent);

        var storagePath = _configuration["StoragePath"] ?? "storage";
        var resumesPath = Path.Combine(storagePath, "resumes", request.UserId.ToString());
        Directory.CreateDirectory(resumesPath);

        var resumeId = Guid.NewGuid();
        var fileName = $"{resumeId}.pdf";
        var filePath = Path.Combine(resumesPath, fileName);
        await File.WriteAllBytesAsync(filePath, pdfBytes, cancellationToken);

        var relativeFilePath = Path.Combine("resumes", request.UserId.ToString(), fileName);
        var generatedResumeResult = GeneratedResume.Create(
            request.UserId,
            request.InternshipId,
            relativeFilePath);

        if (generatedResumeResult.IsFailure)
            throw new InvalidOperationException(generatedResumeResult.Error.Description ??
                                                "Failed to create GeneratedResume entity");

        generatedResumeResult.Value.Id = resumeId;
        var addResult = await _resumeRepository.AddAsync(generatedResumeResult.Value);

        if (!addResult)
            throw new InvalidOperationException("Failed to save GeneratedResume entity to database");

        var downloadUrl = $"/files/{relativeFilePath}";
        var updateResult = studentProfile.UpdateResumeUrl(downloadUrl);

        if (updateResult.IsFailure)
            throw new InvalidOperationException(updateResult.Error.Description);

        await _studentProfileRepository.UpdateAsync(studentProfile);

        return new GenerateResumeResponse(
            resumeId,
            relativeFilePath,
            downloadUrl);
    }
}