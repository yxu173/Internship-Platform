using System.Security.AccessControl;

namespace Application.Features.Roadmaps.DTOs;

public record ResourceLinkDto(
    string Title,
    string Url,
    string Type
);