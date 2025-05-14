using System;
using System.Collections.Generic;
using System.IO;
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
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text(resumeContent.Title)
                        .Style(ResumeStyles.TitleStyle);

                    column.Item().AlignCenter().PaddingVertical(10).Text(text =>
                    {
                        text.Span("Email: ").SemiBold().FontColor("#2c5282");
                        text.Span("student@example.com").Style(ResumeStyles.ContactInfoStyle);
                        text.Span(" | Phone: ").SemiBold().FontColor("#2c5282");
                        text.Span(resumeContent.PhoneNumber).Style(ResumeStyles.ContactInfoStyle);
                        if (!string.IsNullOrEmpty(resumeContent.Location))
                        {
                            text.Span(" | Location: ").SemiBold().FontColor("#2c5282");
                            text.Span(resumeContent.Location).Style(ResumeStyles.ContactInfoStyle);
                        }
                    });
                    
                    column.Item().PaddingVertical(5).LineHorizontal(1).LineColor("#CBD5E0");
                });

                page.Content().Column(column =>
                {
                    // Summary Section
                    column.Item().PaddingBottom(10).Column(c =>
                    {
                        c.Item().Text("PROFESSIONAL SUMMARY").Style(ResumeStyles.HeadingStyle);
                        c.Item().PaddingTop(5).Text(resumeContent.Summary).Style(ResumeStyles.NormalTextStyle);
                    });

                    if (resumeContent.Experience != null && resumeContent.Experience.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().Text("EXPERIENCE").Style(ResumeStyles.HeadingStyle);
                            c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor("#E2E8F0");
                            c.Spacing(8);

                            foreach (var exp in resumeContent.Experience)
                            {
                                c.Item().Column(ec =>
                                {
                                    ec.Item().Text(text =>
                                    {
                                        text.Span($"{exp.Role}").Style(ResumeStyles.SubheadingStyle);
                                        text.Span($" at {exp.Company}").Style(ResumeStyles.NormalTextStyle);
                                        if (!string.IsNullOrEmpty(exp.Duration))
                                        {
                                            text.Span($" | {exp.Duration}").FontColor("#718096").Italic();
                                        }
                                    });

                                    ec.Item().PaddingVertical(2);
                                    
                                    foreach (var bullet in exp.Description)
                                    {
                                        ec.Item().PaddingLeft(10).Text($"• {bullet}").Style(ResumeStyles.BulletPointStyle);
                                    }
                                });
                                
                                c.Item().PaddingBottom(5);
                            }
                        });
                    }

                    if (resumeContent.Projects != null && resumeContent.Projects.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().Text("PROJECTS").Style(ResumeStyles.HeadingStyle);
                            c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor("#E2E8F0");
                            c.Spacing(8);

                            foreach (var project in resumeContent.Projects)
                            {
                                c.Item().Column(pc =>
                                {
                                    pc.Item().Text(project.Title).Style(ResumeStyles.SubheadingStyle);
                                    pc.Item().PaddingVertical(2);
                                    
                                    foreach (var bullet in project.Description)
                                    {
                                        pc.Item().PaddingLeft(10).Text($"• {bullet}").Style(ResumeStyles.BulletPointStyle);
                                    }
                                });
                                
                                c.Item().PaddingBottom(5);
                            }
                        });
                    }

                    column.Item().PaddingVertical(10).Column(c =>
                    {
                        c.Item().Text("SKILLS").Style(ResumeStyles.HeadingStyle);
                        c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor("#E2E8F0");
                        
                        c.Item().PaddingTop(5).Element(container =>
                        {
                            var skillCount = resumeContent.Skills.Count;
                            var columns = skillCount > 8 ? 3 : 2;
                            var skillsPerColumn = (int)Math.Ceiling(skillCount / (double)columns);
                            
                            container.Row(row => 
                            {
                                for (int col = 0; col < columns; col++)
                                {
                                    row.RelativeItem().Column(colContainer =>
                                    {
                                        for (int i = col * skillsPerColumn; i < Math.Min((col + 1) * skillsPerColumn, skillCount); i++)
                                        {
                                            if (i < resumeContent.Skills.Count)
                                            {
                                                colContainer.Item().Text($"• {resumeContent.Skills[i]}").Style(ResumeStyles.BulletPointStyle);
                                            }
                                        }
                                    });
                                }
                            });
                        });
                    });

                    // Education Section
                    column.Item().PaddingVertical(10).Column(c =>
                    {
                        c.Item().Text("EDUCATION").Style(ResumeStyles.HeadingStyle);
                        c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor("#E2E8F0");
                        c.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span($"{resumeContent.Education.Degree}").Style(ResumeStyles.SubheadingStyle);
                            text.Span($", {resumeContent.Education.University}, {resumeContent.Education.GradYear}").Style(ResumeStyles.NormalTextStyle);
                        });

                        if (resumeContent.Education.Highlights != null && resumeContent.Education.Highlights.Count > 0)
                        {
                            c.Item().PaddingVertical(3);
                            foreach (var highlight in resumeContent.Education.Highlights)
                            {
                                c.Item().PaddingLeft(10).Text($"• {highlight}").Style(ResumeStyles.BulletPointStyle);
                            }
                        }
                    });
                });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Generated on ").FontSize(8).FontColor("#A0AEC0");
                        x.Span($"{DateTime.Now:MMMM dd, yyyy}").FontSize(8).FontColor("#A0AEC0");
                    });
                    
            
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }
}
