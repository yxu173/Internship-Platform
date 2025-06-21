using System.Text;
using System.Text.Json;
using Application.Abstractions.Services;
using Application.Features.ResumeGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Services;

public class GeminiAIService : IGeminiAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GeminiAIService> _logger;
    private readonly string _apiKey;
    private readonly string _baseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro-preview-06-05:generateContent";

    public GeminiAIService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiAIService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = _configuration["GeminiAI:ApiKey"] ?? _configuration["GEMINI_API_KEY"] ?? throw new InvalidOperationException("Gemini API key not configured");
    }

    public async Task<Result<string>> GenerateResumeMarkdownAsync(ResumeDto resumeData, CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildResumePrompt(resumeData);
            _logger.LogInformation("Generated prompt for resume generation");
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        role = "user",
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                tools = new[]
                {
                    new
                    {
                        functionDeclarations = new[]
                        {
                            new
                            {
                                name = "generate_resume_markdown",
                                description = "Generates professional resume content in markdown format",
                                parameters = new
                                {
                                    type = "object",
                                    properties = new
                                    {
                                        markdown = new
                                        {
                                            type = "string",
                                            description = "Complete markdown content for a professional resume, including proper formatting, sections, and structure"
                                        }
                                    },
                                    required = new[] { "markdown" }
                                }
                            }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    topP = 0.8,
                    topK = 40
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending request to Gemini AI");
            var response = await _httpClient.PostAsync($"{_baseUrl}?key={_apiKey}", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Gemini AI request failed with status {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
                return Result.Failure<string>(Error.Problem("GeminiAI.Error", $"Failed to generate resume markdown: {errorContent}"));
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogInformation("Received response from Gemini AI");
            
            // Use case-insensitive deserialization to correctly map JSON properties
            var deserializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var responseData = JsonSerializer.Deserialize<GeminiResponse>(responseContent, deserializeOptions);

            if (responseData?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.FunctionCall == null)
            {
                _logger.LogError("No function call response received from Gemini AI. Response: {Response}", responseContent);
                return Result.Failure<string>(Error.Problem("GeminiAI.Error", "No function call response received"));
            }

            var functionCall = responseData.Candidates.First().Content.Parts.First().FunctionCall;
            _logger.LogInformation("Parsing function call arguments");
            
            if (functionCall.Args.ValueKind == JsonValueKind.Null || functionCall.Args.ValueKind == JsonValueKind.Undefined)
            {
                _logger.LogError("No arguments in function call response");
                return Result.Failure<string>(Error.Problem("GeminiAI.Error", "No arguments in function call response"));
            }

            // Parse the args JSON to get the markdown content
            var argsJson = JsonSerializer.Serialize(functionCall.Args);
            var argsData = JsonSerializer.Deserialize<FunctionArgs>(argsJson, deserializeOptions);
            
            var markdownContent = argsData?.Markdown;
            
            if (string.IsNullOrEmpty(markdownContent))
            {
                _logger.LogError("Empty markdown content received from Gemini AI");
                return Result.Failure<string>(Error.Problem("GeminiAI.Error", "Empty markdown content received"));
            }

            _logger.LogInformation("Successfully generated markdown content");
            return Result.Success(markdownContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating resume markdown");
            return Result.Failure<string>(Error.Problem("GeminiAI.Error", $"Error generating resume markdown: {ex.Message}"));
        }
    }

    private string BuildResumePrompt(ResumeDto resumeData)
    {
        var prompt = new StringBuilder();
        
        prompt.AppendLine("You are an expert resume writer and markdown specialist. Create a professional, modern resume in markdown format.");
        prompt.AppendLine();
        prompt.AppendLine("STUDENT INFORMATION:");
        prompt.AppendLine($"Name: {resumeData.StudentName}");
        prompt.AppendLine($"University: {resumeData.University}");
        prompt.AppendLine($"Faculty: {resumeData.Faculty}");
        prompt.AppendLine($"Graduation Year: {resumeData.GraduationYear}");
        prompt.AppendLine($"Phone: {resumeData.PhoneNumber}");
        prompt.AppendLine($"Location: {resumeData.Location}");
        prompt.AppendLine($"Bio: {resumeData.Bio}");
        prompt.AppendLine();
        
        prompt.AppendLine("INTERNSHIP DETAILS:");
        prompt.AppendLine($"Title: {resumeData.InternshipTitle}");
        prompt.AppendLine($"Description: {resumeData.InternshipDescription}");
        prompt.AppendLine($"Requirements: {resumeData.InternshipRequirements}");
        prompt.AppendLine($"Responsibilities: {resumeData.InternshipResponsibilities}");
        prompt.AppendLine();
        
        prompt.AppendLine("COMPANY INFORMATION:");
        prompt.AppendLine($"Company: {resumeData.CompanyName}");
        prompt.AppendLine($"Industry: {resumeData.CompanyIndustry}");
        prompt.AppendLine($"Description: {resumeData.CompanyDescription}");
        prompt.AppendLine();
        
        prompt.AppendLine("STUDENT SKILLS:");
        foreach (var skill in resumeData.Skills)
        {
            prompt.AppendLine($"- {skill.Name}");
        }
        prompt.AppendLine();
        
        prompt.AppendLine("STUDENT EXPERIENCE:");
        foreach (var exp in resumeData.Experiences)
        {
            prompt.AppendLine($"- {exp.Title} at {exp.Company} ({exp.StartDate:MMM yyyy} - {(exp.EndDate?.ToString("MMM yyyy") ?? "Present")})");
        }
        prompt.AppendLine();
        
        prompt.AppendLine("STUDENT PROJECTS:");
        foreach (var project in resumeData.Projects)
        {
            prompt.AppendLine($"- {project.Title}: {project.Description}");
            if (!string.IsNullOrEmpty(project.Link))
            {
                prompt.AppendLine($"  Link: {project.Link}");
            }
        }
        prompt.AppendLine();
        
        prompt.AppendLine("REQUIREMENTS:");
        prompt.AppendLine("1. Create a professional resume in markdown format that highlights the student's relevant skills and experience for this specific internship");
        prompt.AppendLine("2. Use proper markdown formatting: headers (# ## ###), lists (- *), bold (**text**), italic (*text*)");
        prompt.AppendLine("3. Include sections for: Header (name, contact), Summary, Experience, Projects, Skills, Education");
        prompt.AppendLine("4. Make the content compelling and relevant to the internship position");
        prompt.AppendLine("5. Use clean, professional formatting that will look good when converted to PDF");
        prompt.AppendLine("6. Ensure all dates are formatted consistently");
        prompt.AppendLine("7. Make the resume tailored to the specific internship requirements");
        prompt.AppendLine();
        prompt.AppendLine("Please use the generate_resume_markdown function to create the complete markdown content.");

        return prompt.ToString();
    }

    // Response models for Gemini API
    private class GeminiResponse
    {
        public List<Candidate> Candidates { get; set; } = new();
    }

    private class Candidate
    {
        public Content Content { get; set; } = new();
    }

    private class Content
    {
        public List<Part> Parts { get; set; } = new();
    }

    private class Part
    {
        public FunctionCall FunctionCall { get; set; } = new();
    }

    private class FunctionCall
    {
        public string Name { get; set; } = string.Empty;
        public JsonElement Args { get; set; }
    }

    private class FunctionArgs
    {
        public string Markdown { get; set; } = string.Empty;
    }
} 