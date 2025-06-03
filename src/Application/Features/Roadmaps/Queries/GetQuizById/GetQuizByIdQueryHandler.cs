using Application.Abstractions.Messaging;
using Application.Features.Roadmaps.DTOs;
using Domain.DomainErrors;
using Domain.Repositories;
using SharedKernel;

namespace Application.Features.Roadmaps.Queries.GetQuizById;

public sealed class GetQuizByIdQueryHandler : IQueryHandler<GetQuizByIdQuery, QuizDto>
{
    private readonly IRoadmapRepository _roadmapRepository;

    public GetQuizByIdQueryHandler(IRoadmapRepository roadmapRepository)
    {
        _roadmapRepository = roadmapRepository;
    }

    public async Task<Result<QuizDto>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var section = await _roadmapRepository.GetSectionByIdAsync(request.SectionId, includeQuiz: true);
        
        if (section == null)
        {
            return Result.Failure<QuizDto>(RoadmapErrors.SectionNotFound);
        }
        
        if (section.RoadmapId != request.RoadmapId)
        {
            return Result.Failure<QuizDto>(RoadmapErrors.SectionNotFound);
        }

        if (section.Quiz == null || section.Quiz.Id != request.QuizId)
        {
            return Result.Failure<QuizDto>(RoadmapErrors.QuizNotFound);
        }

        var questionsDto = section.Quiz.Questions.Select(q => new QuizQuestionDto(
            q.Id,
            q.Text,
            q.Points,
            q.Options.Select(o => new QuizOptionDto(
                o.Id,
                o.Text,
                o.IsCorrect
            )).ToList()
        )).ToList();

        var quizDto = new QuizDto(
            section.Quiz.Id,
            section.Id,
            section.Title,
            section.Quiz.PassingScore,
            section.Quiz.IsRequired,
            questionsDto
        );

        return Result.Success(quizDto);
    }
} 