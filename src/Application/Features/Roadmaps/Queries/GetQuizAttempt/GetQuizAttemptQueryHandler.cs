using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetQuizAttempt;

public sealed class GetQuizAttemptQueryHandler : IQueryHandler<GetQuizAttemptQuery, GetQuizAttemptResponse>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetQuizAttemptQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<GetQuizAttemptResponse>> Handle(GetQuizAttemptQuery request,
        CancellationToken cancellationToken)
    {
        var quizAttempt = await _roadmapRepository.GetLatestQuizAttemptAsync(request.UserId, request.QuizId);

        if (quizAttempt is null)
        {
            return Result.Failure<GetQuizAttemptResponse>(RoadmapErrors.QuizAttemptNotFound);
        }

        var quiz = await _roadmapRepository.GetQuizByIdAsync(quizAttempt.QuizId);
        if (quiz is null)
        {
            return Result.Failure<GetQuizAttemptResponse>(RoadmapErrors.QuizNotFound);
        }

        var results = quiz.Questions.Select(q =>
        {
            var answer = quizAttempt.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
            return new QuestionResultDto(
                q.Id,
                q.Text,
                q.Points,
                answer?.IsCorrect ?? false,
                answer?.SelectedOptionId ?? Guid.Empty,
                q.Options.Select(o => new OptionDto(o.Id, o.Text, o.IsCorrect)).ToList()
            );
        }).ToList();

        var response = new GetQuizAttemptResponse
        (
            quizAttempt.Id,
            quizAttempt.EnrollmentId,
            quizAttempt.QuizId,
            quizAttempt.Score,
            quiz.CalculateTotalPoints(),
            quiz.PassingScore,
            quizAttempt.Passed,
            quizAttempt.CreatedAt,
            quizAttempt.ModifiedAt,
            results
        );
        return Result.Success(response);
    }
}