using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.Aggregates.Roadmaps;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Commands.SubmitQuizAttempt;

public sealed class SubmitQuizAttemptCommandHandler : ICommandHandler<SubmitQuizAttemptCommand, QuizResultDto>
{
    private readonly IRoadmapRepository _roadmapRepository;
    private readonly IStudentRepository _studentRepository;

    public SubmitQuizAttemptCommandHandler(IRoadmapRepository roadmapRepository, IStudentRepository studentRepository)
    {
        _roadmapRepository = roadmapRepository;
        _studentRepository = studentRepository;
    }

    public async Task<Result<QuizResultDto>> Handle(
        SubmitQuizAttemptCommand request,
        CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByUserIdAsync(request.UserId);
        if (student == null)
        {
            return Result.Failure<QuizResultDto>(Error.NotFound("Student.NotFound", "Student profile not found."));
        }
        
        var section = await _roadmapRepository.GetSectionByIdAsync(request.SectionId, includeQuiz: true);
        if (section == null)
        {
            return Result.Failure<QuizResultDto>(RoadmapErrors.SectionNotFound);
        }
        
        if (section.RoadmapId != request.RoadmapId)
        {
            return Result.Failure<QuizResultDto>(RoadmapErrors.SectionNotFound);
        }

        if (section.Quiz == null || section.Quiz.Id != request.QuizId)
        {
            return Result.Failure<QuizResultDto>(RoadmapErrors.QuizNotFound);
        }

        var enrollment = await _roadmapRepository.GetEnrollmentAsync(student.Id, request.RoadmapId);
        if (enrollment == null)
        {
            return Result.Failure<QuizResultDto>(RoadmapErrors.EnrollmentNotFound);
        }
        
        var sectionProgress = enrollment.SectionProgress.FirstOrDefault(sp => sp.SectionId == section.Id);
    
        var attemptResult = enrollment.StartQuizAttempt(section.Quiz.Id, section.Id);
        if (attemptResult.IsFailure)
        {
            return Result.Failure<QuizResultDto>(attemptResult.Error);
        }

        var attempt = attemptResult.Value;
        
        var questionResults = new List<QuestionResultDto>();
        foreach (var answer in request.Answers)
        {
            var question = section.Quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question == null) continue;

            var isCorrect = question.IsCorrectOption(answer.SelectedOptionId);
            
            attempt.AddAnswer(answer.QuestionId, answer.SelectedOptionId, isCorrect, question.Points);
            
            var options = question.Options.Select(o => new OptionDto(
                o.Id,
                o.Text,
                o.IsCorrect
            )).ToList();
            
            questionResults.Add(new QuestionResultDto(
                question.Id,
                question.Text,
                question.Points,
                isCorrect,
                answer.SelectedOptionId,
                options
            ));
        }

        var recordResult = enrollment.RecordQuizResult(attempt, section.Quiz);
        if (recordResult.IsFailure)
        {
            return Result.Failure<QuizResultDto>(recordResult.Error);
        }

        await _roadmapRepository.UpdateEnrollmentAsync(enrollment);

        var roadmap = await _roadmapRepository.GetByIdAsync(request.RoadmapId, includeSections: true);
        if (roadmap == null)
        {
            return Result.Failure<QuizResultDto>(RoadmapErrors.RoadmapNotFound);
        }

        var orderedSections = roadmap.Sections.OrderBy(s => s.Order).ToList();
        var nextSectionIndex = orderedSections.FindIndex(s => s.Id == section.Id) + 1;
        var nextSectionUnlocked = nextSectionIndex < orderedSections.Count && 
                                 (attempt.Passed || !section.Quiz.IsRequired);

        var quizResult = new QuizResultDto(
            section.Quiz.Id,
            attempt.Id,
            attempt.Score,
            section.Quiz.CalculateTotalPoints(),
            section.Quiz.PassingScore,
            attempt.Passed,
            nextSectionUnlocked,
            questionResults
        );

        return Result.Success(quizResult);
    }
} 