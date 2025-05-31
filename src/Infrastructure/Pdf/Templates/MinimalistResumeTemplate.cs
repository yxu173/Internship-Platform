using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using System;
using System.Drawing;

namespace Infrastructure.Pdf.Templates
{
    public class MinimalistResumeTemplate : IResumeTemplate
    {
        private readonly ResumeContent _content;
        private readonly ResumeStyles _styles;

        public MinimalistResumeTemplate(ResumeContent content, ResumeStyles styles)
        {
            _content = content;
            _styles = styles;
        }

        public void ComposeDocument(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(_styles.MainFont));

                // Minimal header with just name and title
                page.Header().Padding(10).Column(column =>
                {
                    // Name and title with minimal styling
                    if (_content.Title.Contains("-"))
                    {
                        var parts = _content.Title.Split('-');
                        
                        column.Item().Text(parts[0].Trim())
                            .Style(TextStyle.Default.FontSize(20).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                        
                        if (parts.Length > 1)
                        {
                            column.Item().Text(parts[1].Trim())
                                .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetText()).FontFamily(_styles.HeaderFont));
                        }
                    }
                    else
                    {
                        column.Item().Text(_content.Title)
                            .Style(TextStyle.Default.FontSize(20).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                    }
                    
                    // Contact info in a single line
                    column.Item().PaddingTop(5).Text(text =>
                    {
                        text.Span("student@example.com").Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                        text.Span(" • ").Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                        text.Span(_content.PhoneNumber).Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                        
                        if (!string.IsNullOrEmpty(_content.Location))
                        {
                            text.Span(" • ").Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                            text.Span(_content.Location).Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                        }
                    });
                    
                    column.Item().PaddingTop(10).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                });

                // Main content with minimalist design
                page.Content().PaddingTop(20).Column(column =>
                {
                    // Summary
                    column.Item().Text(_content.Summary)
                        .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()).Italic());
                    
                    column.Item().PaddingVertical(15);
                    
                    // Experience
                    if (_content.Experience != null && _content.Experience.Count > 0)
                    {
                        column.Item().Text("Experience")
                            .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).FontFamily(_styles.HeaderFont).SemiBold());
                            
                        column.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        column.Item().PaddingVertical(8);
                        
                        foreach (var exp in _content.Experience)
                        {
                            column.Item().Column(c =>
                            {
                                c.Item().Row(r => 
                                {
                                    r.RelativeItem().Text(exp.Role)
                                        .Style(TextStyle.Default.FontSize(11).FontColor(_styles.ColorScheme.GetTertiary()).SemiBold());
                                        
                                    r.ConstantItem(100).AlignRight().Text(exp.Duration)
                                        .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()).Italic());
                                });
                                
                                c.Item().Text(exp.Company)
                                    .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                    
                                c.Item().PaddingTop(5);
                                
                                foreach (var bullet in exp.Description)
                                {
                                    c.Item().Text(bullet)
                                        .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                                }
                                
                                c.Item().PaddingBottom(10);
                            });
                        }
                    }
                    
                    // Projects
                    if (_content.Projects != null && _content.Projects.Count > 0)
                    {
                        column.Item().PaddingTop(5).Text("Projects")
                            .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).FontFamily(_styles.HeaderFont).SemiBold());
                            
                        column.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                        column.Item().PaddingVertical(8);
                        
                        foreach (var project in _content.Projects)
                        {
                            column.Item().Column(c =>
                            {
                                c.Item().Text(project.Title)
                                    .Style(TextStyle.Default.FontSize(11).FontColor(_styles.ColorScheme.GetTertiary()).SemiBold());
                                    
                                c.Item().PaddingTop(5);
                                
                                foreach (var bullet in project.Description)
                                {
                                    c.Item().Text(bullet)
                                        .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                                }
                                
                                c.Item().PaddingBottom(10);
                            });
                        }
                    }
                    
                    // Skills and Education in a single row at the bottom
                    column.Item().PaddingTop(5).Row(row =>
                    {
                        // Skills section
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Skills")
                                .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).FontFamily(_styles.HeaderFont).SemiBold());
                                
                            c.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                            c.Item().PaddingVertical(8);
                            
                            var skillGroups = (int)Math.Ceiling(_content.Skills.Count / 2.0);
                            
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Column(col =>
                                {
                                    for (int i = 0; i < Math.Min(skillGroups, _content.Skills.Count); i++)
                                    {
                                        col.Item().Text(_content.Skills[i])
                                            .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                                    }
                                });
                                
                                if (_content.Skills.Count > skillGroups)
                                {
                                    r.RelativeItem().Column(col =>
                                    {
                                        for (int i = skillGroups; i < _content.Skills.Count; i++)
                                        {
                                            col.Item().Text(_content.Skills[i])
                                                .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                                        }
                                    });
                                }
                            });
                        });
                        
                        // Space between columns
                        row.ConstantItem(20);
                        
                        // Education section
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Education")
                                .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).FontFamily(_styles.HeaderFont).SemiBold());
                                
                            c.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten3);
                            c.Item().PaddingVertical(8);
                            
                            c.Item().Text(_content.Education.Degree)
                                .Style(TextStyle.Default.FontSize(11).FontColor(_styles.ColorScheme.GetTertiary()).SemiBold());
                                
                            c.Item().Text(_content.Education.University)
                                .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                
                            c.Item().Text(_content.Education.GradYear)
                                .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()).Italic());
                                
                            if (_content.Education.Highlights != null && _content.Education.Highlights.Count > 0)
                            {
                                c.Item().PaddingTop(5);
                                
                                foreach (var highlight in _content.Education.Highlights)
                                {
                                    c.Item().Text(highlight)
                                        .Style(TextStyle.Default.FontSize(9).FontColor(_styles.ColorScheme.GetText()));
                                }
                            }
                        });
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span($"{DateTime.Now:MMMM dd, yyyy}").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
            });
        }
    }
}
