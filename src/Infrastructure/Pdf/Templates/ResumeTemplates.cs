using System;
using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;

namespace Infrastructure.Pdf.Templates
{
    // Interface for resume templates
    public interface IResumeTemplate
    {
        void ComposeDocument(IDocumentContainer container);
    }
    
    // Factory for creating different resume template implementations
    public static class ResumeTemplateFactory
    {
        private static readonly Dictionary<string, Func<ResumeContent, ResumeStyles, IResumeTemplate>> TemplateMap = 
            new Dictionary<string, Func<ResumeContent, ResumeStyles, IResumeTemplate>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Classic", (content, styles) => new ClassicResumeTemplate(content, styles) },
                { "Modern", (content, styles) => new ModernResumeTemplate(content, styles) },
                { "Creative", (content, styles) => new CreativeResumeTemplate(content, styles) },
                { "Minimalist", (content, styles) => new MinimalistResumeTemplate(content, styles) },
                { "TwoColumn", (content, styles) => new TwoColumnResumeTemplate(content, styles) }
            };

        public static IResumeTemplate CreateTemplate(string templateName, ResumeContent content, ResumeStyles styles)
        {
            if (string.IsNullOrEmpty(templateName) || !TemplateMap.ContainsKey(templateName))
            {
                templateName = "Classic"; // Default template
            }

            return TemplateMap[templateName](content, styles);
        }

        public static IEnumerable<string> GetAvailableTemplates()
        {
            return TemplateMap.Keys;
        }
    }
}
