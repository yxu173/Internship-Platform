using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using System;

namespace Infrastructure.Pdf.Templates
{
    public class ClassicResumeTemplate : IResumeTemplate
    {
        private readonly ResumeContent _content;
        private readonly ResumeStyles _styles;

        public ClassicResumeTemplate(ResumeContent content, ResumeStyles styles)
        {
            _content = content;
            _styles = styles;
        }

        public void ComposeDocument(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(50);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(_styles.MainFont));

                page.Header().Column(column =>
                {
                    column.Item().AlignCenter().Text(_content.Title)
                        .Style(_styles.TitleStyle);

                    column.Item().AlignCenter().PaddingVertical(10).Text(text =>
                    {
                        text.Span("Email: ").SemiBold().FontColor(_styles.ColorScheme.Secondary);
                        text.Span("student@example.com").Style(_styles.ContactInfoStyle);
                        text.Span(" | Phone: ").SemiBold().FontColor(_styles.ColorScheme.Secondary);
                        text.Span(_content.PhoneNumber).Style(_styles.ContactInfoStyle);
                        if (!string.IsNullOrEmpty(_content.Location))
                        {
                            text.Span(" | Location: ").SemiBold().FontColor(_styles.ColorScheme.Secondary);
                            text.Span(_content.Location).Style(_styles.ContactInfoStyle);
                        }
                    });
                    
                    column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(_styles.GetDividerColor());
                });

                page.Content().Column(column =>
                {
                    // Summary Section
                    column.Item().PaddingBottom(10).Column(c =>
                    {
                        c.Item().Text("PROFESSIONAL SUMMARY").Style(_styles.HeadingStyle);
                        c.Item().PaddingTop(5).Text(_content.Summary).Style(_styles.NormalTextStyle);
                    });

                    if (_content.Experience != null && _content.Experience.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().Text("EXPERIENCE").Style(_styles.HeadingStyle);
                            c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                            c.Spacing(8);

                            foreach (var exp in _content.Experience)
                            {
                                c.Item().Column(ec =>
                                {
                                    ec.Item().Text(text =>
                                    {
                                        text.Span($"{exp.Role}").Style(_styles.SubheadingStyle);
                                        text.Span($" at {exp.Company}").Style(_styles.NormalTextStyle);
                                        if (!string.IsNullOrEmpty(exp.Duration))
                                        {
                                            text.Span($" | {exp.Duration}").FontColor("#718096").Italic();
                                        }
                                    });

                                    ec.Item().PaddingVertical(2);
                                    
                                    foreach (var bullet in exp.Description)
                                    {
                                        ec.Item().PaddingLeft(10).Text($"• {bullet}").Style(_styles.BulletPointStyle);
                                    }
                                });
                                
                                c.Item().PaddingBottom(5);
                            }
                        });
                    }

                    if (_content.Projects != null && _content.Projects.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().Text("PROJECTS").Style(_styles.HeadingStyle);
                            c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                            c.Spacing(8);

                            foreach (var project in _content.Projects)
                            {
                                c.Item().Column(pc =>
                                {
                                    pc.Item().Text(project.Title).Style(_styles.SubheadingStyle);
                                    pc.Item().PaddingVertical(2);
                                    
                                    foreach (var bullet in project.Description)
                                    {
                                        pc.Item().PaddingLeft(10).Text($"• {bullet}").Style(_styles.BulletPointStyle);
                                    }
                                });
                                
                                c.Item().PaddingBottom(5);
                            }
                        });
                    }

                    column.Item().PaddingVertical(10).Column(c =>
                    {
                        c.Item().Text("SKILLS").Style(_styles.HeadingStyle);
                        c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                        
                        c.Item().PaddingTop(5).Element(container =>
                        {
                            var skillCount = _content.Skills.Count;
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
                                            if (i < _content.Skills.Count)
                                            {
                                                colContainer.Item().Text($"• {_content.Skills[i]}").Style(_styles.BulletPointStyle);
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
                        c.Item().Text("EDUCATION").Style(_styles.HeadingStyle);
                        c.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                        c.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span($"{_content.Education.Degree}").Style(_styles.SubheadingStyle);
                            text.Span($", {_content.Education.University}, {_content.Education.GradYear}").Style(_styles.NormalTextStyle);
                        });

                        if (_content.Education.Highlights != null && _content.Education.Highlights.Count > 0)
                        {
                            c.Item().PaddingVertical(3);
                            foreach (var highlight in _content.Education.Highlights)
                            {
                                c.Item().PaddingLeft(10).Text($"• {highlight}").Style(_styles.BulletPointStyle);
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
        }
    }
}
