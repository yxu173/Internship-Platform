using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.AI.Gemini;

public class GeminiClient : IGeminiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly ILogger<GeminiClient> _logger;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public GeminiClient(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiClient> logger = null)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
        _apiKey = configuration["GEMINI_API_KEY"] ??
                  throw new InvalidOperationException("GEMINI_API_KEY is not configured");
        _modelName = configuration["GEMINI_MODEL_NAME"] ?? "gemini-1.5-pro";
        _logger = logger;
    }

    public async Task<T> InvokeFunctionAsync<T>(string functionName, object functionParameters,
        byte[] templateFile = null, CancellationToken cancellationToken = default)
    {
        var systemInstruction =
            "You are a professional resume writer and designer with expertise in crafting compelling resumes for students applying to internships. " +
            "Your task is to generate resume content and select appropriate styling that will help the student stand out to employers, ensuring the content is concise and designed to fit within a single page. " +
            "When a resume template is provided, analyze its structure, layout, typography, and color scheme carefully. " +
            "Your generated resume content should follow a similar design philosophy and structure as the template, focusing on the most relevant information. " +
            "Pay attention to how the template organizes sections, uses white space, font combinations, and visual hierarchy. " +
            "Focus on highlighting relevant skills, experience, and projects that align with the internship requirements.";

        // Extract available themes, templates, and fonts information to include in the prompt
        var optionsInfo = JsonSerializer.Serialize(new
        {
            Templates = new[]
            {
                new { Name = "Classic", Description = "Traditional resume layout with clean sections organized vertically" },
                new { Name = "Modern", Description = "Contemporary design with colored section headers and modern typography" },
                new { Name = "Creative", Description = "Distinctive design with visual elements, icons, and creative section layouts" },
                new { Name = "Minimalist", Description = "Clean, simple design with minimal elements and focused content" },
                new { Name = "TwoColumn", Description = "Efficient two-column layout with sidebar for skills and education" }
            },
            Themes = new[]
            {
                new { Name = "Professional", Description = "Blue-based color scheme with a corporate feel, suitable for traditional industries" },
                new { Name = "Modern", Description = "Dark neutral colors with a clean, contemporary look" },
                new { Name = "Creative", Description = "Purple-focused palette with vibrant accents, ideal for creative roles" },
                new { Name = "Bold", Description = "Red-based palette with strong accents, makes a statement" },
                new { Name = "Elegant", Description = "Teal and blue-green tones with a sophisticated feel" },
                new { Name = "Corporate", Description = "Deep blue professional color scheme for formal business environments" },
                new { Name = "Minimalist", Description = "Subtle grayscale with minimal color for a clean, simple look" },
            },
            Fonts = new[]
            {
                "Arial", "Helvetica", "Times New Roman", "Calibri", "Georgia", "Garamond",
                "Verdana", "Roboto", "Tahoma", "Trebuchet MS"
            }
        }, _jsonOptions);

        var promptText = templateFile != null ?
            "I'm providing a resume template PDF along with student profile and internship details. " +
            "Please analyze BOTH the template design AND the student data to generate a professional resume. " +
            "Study the template's layout, typography, spacing, and overall aesthetic. Note the fonts used for headings and body text, " +
            "the color scheme, how different sections are organized, and any other design elements. " +
            "Then create resume content that would fit this exact template style while highlighting skills and experiences " +
            "that match the internship requirements. Focus on content that will help the student secure this specific opportunity.\n\n" +
            "As part of your response, please select both a template design layout AND appropriate styling for the resume. " +
            "First select which template layout best fits the student's profile, then choose a color theme and fonts or create a custom color scheme. " +
            "Your choices should complement the student's field, career stage, and the specific internship.\n\n" +
            "Available options (templates, themes, and fonts):\n" + optionsInfo + "\n\n" +
            "You may also create a custom color scheme by defining specific colors for different resume elements.\n\n" +
            "Student and Internship data:\n" +
            JsonSerializer.Serialize(functionParameters, _jsonOptions) :
            
            "Please analyze the following student profile and internship details to generate a professional resume. " +
            "The resume should highlight skills and experiences that match the internship requirements. " +
            "Create a concise, well-structured resume that will help the student secure this internship opportunity.\n\n" +
            "As part of your response, please select both a template design layout AND appropriate styling for the resume. " +
            "First select which template layout best fits the student's profile, then choose a color theme and fonts or create a custom color scheme. " +
            "Your choices should complement the student's field, career stage, and the specific internship.\n\n" +
            "Available options (templates, themes, and fonts):\n" + optionsInfo + "\n\n" +
            "You may also create a custom color scheme by defining specific colors for different resume elements.\n\n" +
            "Student and Internship data:\n" +
            JsonSerializer.Serialize(functionParameters, _jsonOptions);

        object filePart = null;
        if (templateFile != null)
        {   
            filePart = new
            {
                inlineData = new
                {
                    mimeType = "application/pdf",
                    data = Convert.ToBase64String(templateFile)
                }
            };
        }
            
        var requestContent = new
        {
            system_instruction = new
            {
                parts = new[]
                {
                    new { text = systemInstruction }
                }
            },
            contents = new[]
            {
                new
                {
                    parts = filePart != null ? 
                        new[] { new { text = promptText }, filePart } :
                        new[] { new { text = promptText } }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                topP = 0.7,
                topK = 5,
                thinkingConfig = new { thinkingBudget = 1024 }
            },
            tools = new[]
            {
                new
                {
                    functionDeclarations = new[]
                    {
                        GetFunctionDeclaration(functionName)
                    }
                }
            },
            toolConfig = new
            {
                functionCallingConfig = new
                {
                    mode = "ANY", 
                    allowedFunctionNames = new[] { functionName }
                }
            },
            safetySettings = new[]
            {
                new { category = "HARM_CATEGORY_HARASSMENT", threshold = "BLOCK_NONE" },
                new { category = "HARM_CATEGORY_HATE_SPEECH", threshold = "BLOCK_NONE" },
                new { category = "HARM_CATEGORY_SEXUALLY_EXPLICIT", threshold = "BLOCK_NONE" },
                new { category = "HARM_CATEGORY_DANGEROUS_CONTENT", threshold = "BLOCK_NONE" }
            }
        };

        var requestJson = JsonSerializer.Serialize(requestContent, _jsonOptions);
        var url = $"models/{_modelName}:generateContent?key={_apiKey}";

        _logger?.LogInformation("Sending request to Gemini API for function {FunctionName}", functionName);

        HttpResponseMessage response = null;
        int maxRetries = 3;
        int currentRetry = 0;
        bool succeeded = false;

        _httpClient.Timeout = TimeSpan.FromSeconds(300);
        while (currentRetry < maxRetries && !succeeded)
        {
            try
            {
                _logger?.LogDebug("Attempt {Attempt} calling Gemini API", currentRetry + 1);
                var httpContent = new StringContent(requestJson, Encoding.UTF8, "application/json");
                response = await _httpClient.PostAsync(url, httpContent, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    succeeded = true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger?.LogWarning("Gemini API returned error status code: {StatusCode}, Content: {ErrorContent}",
                        response.StatusCode, errorContent);

                    if ((int)response.StatusCode >= 500 ||
                        response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        currentRetry++;
                        await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, currentRetry)),
                            cancellationToken); // Exponential backoff
                    }
                    else
                    {
                        throw new HttpRequestException(
                            $"Gemini API request failed with status code {response.StatusCode}: {errorContent}");
                    }
                }
            }
            catch (HttpRequestException ex) when (currentRetry < maxRetries - 1)
            {
                _logger?.LogWarning(ex, "HTTP request to Gemini API failed, retrying ({RetryCount}/{MaxRetries})",
                    currentRetry + 1, maxRetries);
                currentRetry++;
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, currentRetry)), cancellationToken);
            }
            catch (TaskCanceledException ex) when (cancellationToken.IsCancellationRequested)
            {
                _logger?.LogWarning("Gemini API request was cancelled");
                throw;
            }
        }

        if (!succeeded || response == null)
        {
            throw new InvalidOperationException(
                $"Failed to get successful response from Gemini API after {maxRetries} attempts");
        }

        var responseBody =
            await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken: cancellationToken);

        if (responseBody?.Candidates == null || responseBody.Candidates.Count == 0)
        {
            throw new InvalidOperationException("No response candidates from Gemini API");
        }

        if (responseBody.Candidates[0].FinishReason == "MAX_TOKENS")
        {
            _logger?.LogWarning("Gemini API hit MAX_TOKENS limit. Consider shortening prompt or increasing tokens.");
            throw new InvalidOperationException("MAX_TOKENS limit reached; response incomplete.");
        }

        foreach (var part in responseBody.Candidates[0].Content.Parts)
        {
            if (part.FunctionCall != null)
            {
                _logger?.LogInformation("Received function call response from Gemini for function {FunctionName}",
                    part.FunctionCall.Name);

                return JsonSerializer.Deserialize<T>(part.FunctionCall.Args.ToString(), _jsonOptions);
            }
        }

        _logger?.LogError("No function call found in Gemini API response");
        _logger?.LogInformation("Debug: Full Gemini response content - {Response}", JsonSerializer.Serialize(responseBody, _jsonOptions));
        string rawResponse = JsonSerializer.Serialize(responseBody, _jsonOptions);
        throw new InvalidOperationException($"No function call in response. Raw response: {rawResponse}");
    }

 

    private object GetFunctionDeclaration(string functionName)
    {
        if (functionName == "generate_resume")
        {
            return new
            {
                name = "generate_resume",
                description =
                    "Generate a professional resume using student and internship information. Create a resume that highlights the student's relevant skills and experiences for the target internship position.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        title = new
                        {
                            type = "string",
                            description =
                                "A professional title for the resume, typically includes the student's name and target role"
                        },
                        summary = new
                        {
                            type = "string",
                            description =
                                "A concise professional summary highlighting the student's strengths and suitability for the internship"
                        },
                        experience = new
                        {
                            type = "array",
                            description =
                                "The student's work experiences, formatted and prioritized for relevance to the internship",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    role = new { type = "string", description = "Job title or role" },
                                    company = new { type = "string", description = "Company or organization name" },
                                    duration = new { type = "string", description = "Time period of employment" },
                                    description = new
                                    {
                                        type = "array",
                                        description = "Bullet points describing achievements and responsibilities",
                                        items = new { type = "string" }
                                    }
                                },
                                required = new[] { "role", "company", "description" }
                            }
                        },
                        projects = new
                        {
                            type = "array",
                            description =
                                "The student's projects, selected and described for relevance to the internship",
                            items = new
                            {
                                type = "object",
                                properties = new
                                {
                                    title = new { type = "string", description = "Project name or title" },
                                    description = new
                                    {
                                        type = "array",
                                        description =
                                            "Bullet points describing the project, technologies used, and outcomes",
                                        items = new { type = "string" }
                                    }
                                },
                                required = new[] { "title", "description" }
                            }
                        },
                        skills = new
                        {
                            type = "array",
                            description = "List of skills relevant to the internship, ordered by relevance",
                            items = new { type = "string" }
                        },
                        education = new
                        {
                            type = "object",
                            description = "Educational background information",
                            properties = new
                            {
                                university = new { type = "string", description = "University or institution name" },
                                degree = new { type = "string", description = "Degree and field of study" },
                                gradYear = new
                                    { type = "string", description = "Graduation year or expected graduation" },
                                highlights = new
                                {
                                    type = "array",
                                    description = "Notable academic achievements or relevant coursework",
                                    items = new { type = "string" }
                                }
                            },
                            required = new[] { "university", "degree", "gradYear" }
                        },
                        stylePreferences = new
                        {
                            type = "object",
                            description = "Style preferences for the resume including template design, theme name and font choices",
                            properties = new
                            {
                                templateName = new
                                {
                                    type = "string",
                                    description = "Name of the resume template to use from predefined list: Classic, Modern, Creative, Minimalist, or TwoColumn. Choose a template layout that best showcases the student's experience and fits the internship position."
                                },
                                themeName = new 
                                { 
                                    type = "string", 
                                    description = "Name of the theme to use from predefined list: Professional, Modern, Creative, Bold, Elegant, Corporate, or Minimalist. Choose one that best fits the student's career field and the internship position." 
                                },
                                mainFont = new 
                                { 
                                    type = "string", 
                                    description = "Font family for main text content. Choose one of: Arial, Helvetica, Times New Roman, Calibri, Georgia, Garamond, Verdana, Roboto, Tahoma, or Trebuchet MS." 
                                },
                                headerFont = new 
                                { 
                                    type = "string", 
                                    description = "Font family for headings and titles. Choose one of: Arial, Helvetica, Times New Roman, Calibri, Georgia, Garamond, Verdana, Roboto, Tahoma, or Trebuchet MS." 
                                },
                                customColors = new
                                {
                                    type = "object",
                                    description = "Optional custom color scheme if not using a predefined theme",
                                    properties = new
                                    {
                                        primaryColor = new { type = "string", description = "Main title color in hex format (e.g., #1a365d)" },
                                        secondaryColor = new { type = "string", description = "Section headings color in hex format (e.g., #2c5282)" },
                                        tertiaryColor = new { type = "string", description = "Subheadings color in hex format (e.g., #2d3748)" },
                                        textColor = new { type = "string", description = "Normal text color in hex format (e.g., #4a5568)" },
                                        dividerColor = new { type = "string", description = "Color of horizontal dividers in hex format (e.g., #CBD5E0)" }
                                    }
                                }
                            }
                        }
                    },
                    required = new[] { "title", "summary", "skills", "education", "stylePreferences" }
                }
            };
        }

        throw new NotImplementedException($"Function {functionName} not implemented");
    }
}

public class GeminiResponse
{
    [JsonPropertyName("candidates")] public List<Candidate> Candidates { get; set; }

    [JsonPropertyName("promptFeedback")] public PromptFeedback PromptFeedback { get; set; }
}

public class Candidate
{
    [JsonPropertyName("content")] public Content Content { get; set; }

    [JsonPropertyName("finishReason")] public string FinishReason { get; set; }

    [JsonPropertyName("index")] public int Index { get; set; }
}

public class Content
{
    [JsonPropertyName("parts")] public List<Part> Parts { get; set; }

    [JsonPropertyName("role")] public string Role { get; set; }
}

public class Part
{
    [JsonPropertyName("functionCall")] public FunctionCall FunctionCall { get; set; }
}

public class FunctionCall
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("args")] public JsonElement Args { get; set; }
}

public class PromptFeedback
{
    [JsonPropertyName("safetyRatings")] public List<SafetyRating> SafetyRatings { get; set; }
}

public class SafetyRating
{
    [JsonPropertyName("category")] public string Category { get; set; }

    [JsonPropertyName("probability")] public string Probability { get; set; }
}