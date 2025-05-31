namespace Application.Features.StudentProfile.Queries.GetAllStudentProjects;

public sealed record StudentProjectDto( 
    string ProjectName,
    string Description,
    string? ProjectUrl);