using Application.Features.ResumeGeneration;

namespace Application.Abstractions.Services;

public interface IPdfResumeRenderer
{
    byte[] Render(ResumeContent resumeContent);

}