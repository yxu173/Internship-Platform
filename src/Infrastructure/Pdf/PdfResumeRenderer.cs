using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Abstractions.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using Infrastructure.Pdf.Templates;

namespace Infrastructure.Pdf;

public class PdfResumeRenderer : IPdfResumeRenderer
{
    public byte[] Render(ResumeContent resumeContent)
    {
        // Create styles based on content styling preferences
        var styles = CreateStylesFromContent(resumeContent);
        
        // Get the appropriate template name from content
        string templateName = GetTemplateNameFromContent(resumeContent);
        
        // Create template instance using factory
        var template = ResumeTemplateFactory.CreateTemplate(templateName, resumeContent, styles);
        
        // Generate document using the selected template
        var document = Document.Create(container =>
        {
            template.ComposeDocument(container);
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
    
    private string GetTemplateNameFromContent(ResumeContent content)
    {        
        if (content.StylePreferences != null && !string.IsNullOrEmpty(content.StylePreferences.TemplateName))
        {
            // Get available template names
            var availableTemplates = ResumeTemplateFactory.GetAvailableTemplates().ToList();
            
            // Check if the requested template is available
            if (availableTemplates.Any(t => t.Equals(content.StylePreferences.TemplateName, StringComparison.OrdinalIgnoreCase)))
            {
                return content.StylePreferences.TemplateName;
            }
        }
        
        // Default to classic template if no preference or template not found
        return "Classic";
    }
    
    private ResumeStyles CreateStylesFromContent(ResumeContent content)
    {
        // If styling details are provided in the ResumeContent, use them
        // Otherwise, use default styling
        if (content.StylePreferences != null)
        {
            // Extract theme name if provided, otherwise use default
            string themeName = !string.IsNullOrEmpty(content.StylePreferences.ThemeName) ?
                content.StylePreferences.ThemeName : "Professional";
                
            // Extract font names if provided, otherwise use defaults
            string mainFont = !string.IsNullOrEmpty(content.StylePreferences.MainFont) ? 
                content.StylePreferences.MainFont : "Arial";
            string headerFont = !string.IsNullOrEmpty(content.StylePreferences.HeaderFont) ? 
                content.StylePreferences.HeaderFont : mainFont;
            
            // If custom colors are provided, use them instead of a theme
            if (content.StylePreferences.CustomColors != null)
            {
                var customColors = new ResumeColorScheme
                {
                    Primary = content.StylePreferences.CustomColors.PrimaryColor ?? "#1a365d",
                    Secondary = content.StylePreferences.CustomColors.SecondaryColor ?? "#2c5282",
                    Tertiary = content.StylePreferences.CustomColors.TertiaryColor ?? "#2d3748",
                    Text = content.StylePreferences.CustomColors.TextColor ?? "#4a5568",
                    Divider = content.StylePreferences.CustomColors.DividerColor ?? "#CBD5E0"
                };
                
                return new ResumeStyles(customColors, mainFont, headerFont);
            }
            
            // Otherwise create using theme name
            return new ResumeStyles(themeName, mainFont, headerFont);
        }
        
        // Use default styling if no preferences are provided
        return new ResumeStyles();
    }
}
