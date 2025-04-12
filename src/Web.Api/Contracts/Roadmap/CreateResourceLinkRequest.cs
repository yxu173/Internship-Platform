namespace Web.Api.Contracts.Roadmap;

public record CreateResourceLinkRequest(
    string Title,
    string Url,
    string Type 
); 