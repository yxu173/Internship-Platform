using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using System;
using System.Drawing;

namespace Infrastructure.Pdf.Templates
{
    public class CreativeResumeTemplate : IResumeTemplate
    {
        private readonly ResumeContent _content;
        private readonly ResumeStyles _styles;

        public CreativeResumeTemplate(ResumeContent content, ResumeStyles styles)
        {
            _content = content;
            _styles = styles;
        }

        public void ComposeDocument(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(0);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(_styles.MainFont));

                // Top header with background color
                page.Header().Height(130).Background(_styles.ColorScheme.GetPrimary()).Padding(20).Column(column =>
                {
                    column.Item().Row(row =>
                    {
                        row.RelativeItem(3).Column(c =>
                        {
                            c.Item().Text(_content.Title.Split('-')[0].Trim())
                                .Style(TextStyle.Default.FontSize(24).FontColor("white").FontFamily(_styles.HeaderFont).Bold());
                            
                            if (_content.Title.Contains("-"))
                            {
                                var parts = _content.Title.Split('-');
                                if (parts.Length > 1)
                                {
                                    c.Item().Text(parts[1].Trim())
                                        .Style(TextStyle.Default.FontSize(16).FontColor("white").FontFamily(_styles.HeaderFont).Italic());
                                }
                            }
                        });
                        
                        row.RelativeItem(2).Column(c =>
                        {
                            c.Item().AlignRight().Text(text =>
                            {
                                text.Span("Email: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                                text.Span("student@example.com").Style(TextStyle.Default.FontColor("white"));
                            });
                            
                            c.Item().AlignRight().Text(text =>
                            {
                                text.Span("Phone: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                                text.Span(_content.PhoneNumber).Style(TextStyle.Default.FontColor("white"));
                            });
                            
                            if (!string.IsNullOrEmpty(_content.Location))
                            {
                                c.Item().AlignRight().Text(text =>
                                {
                                    text.Span("Location: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                                    text.Span(_content.Location).Style(TextStyle.Default.FontColor("white"));
                                });
                            }
                        });
                    });
                });

                // Main content with creative design
                page.Content().PaddingVertical(20).Padding(30).Column(column =>
                {
                    // Summary with icon-like bullet
                    column.Item().PaddingBottom(15).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.ConstantItem(20).Height(20).Width(20).Background(_styles.ColorScheme.GetSecondary());
                            r.ConstantItem(10);
                            r.RelativeItem().Text("ABOUT ME").Style(_styles.HeadingStyle);
                        });
                        
                        c.Item().PaddingLeft(30).PaddingTop(5).Text(_content.Summary).Style(_styles.NormalTextStyle);
                    });

                    // Experience section with timeline-like design
                    if (_content.Experience != null && _content.Experience.Count > 0)
                    {
                        column.Item().PaddingVertical(15).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.ConstantItem(20).Height(20).Width(20).Background(_styles.ColorScheme.GetSecondary());
                                r.ConstantItem(10);
                                r.RelativeItem().Text("PROFESSIONAL EXPERIENCE").Style(_styles.HeadingStyle);
                            });
                            
                            c.Item().PaddingLeft(10).Element(container =>
                            {
                                container.Column(ec =>
                                {
                                    foreach (var exp in _content.Experience)
                                    {
                                        ec.Item().PaddingVertical(8).Row(r =>
                                        {
                                            // Timeline design
                                            r.ConstantItem(10).Column(timeCol =>
                                            {
                                                timeCol.Item().AlignCenter().Width(2).Height(100).Background(_styles.ColorScheme.GetTertiary());
                                            });
                                            
                                            r.ConstantItem(20);
                                            
                                            // Experience content 
                                            r.RelativeItem().Column(expCol =>
                                            {
                                                expCol.Item().Text(exp.Role).Style(_styles.SubheadingStyle);
                                                expCol.Item().Text(exp.Company).Style(_styles.NormalTextStyle.SemiBold());
                                                
                                                if (!string.IsNullOrEmpty(exp.Duration))
                                                {
                                                    expCol.Item().Text(exp.Duration)
                                                        .Style(TextStyle.Default.Italic().FontColor(_styles.ColorScheme.Text).FontSize(9));
                                                }
                                                
                                                expCol.Item().PaddingVertical(5);
                                                
                                                foreach (var bullet in exp.Description)
                                                {
                                                    expCol.Item().PaddingLeft(10).Text($"▹ {bullet}").Style(_styles.BulletPointStyle);
                                                }
                                            });
                                        });
                                    }
                                });
                            });
                        });
                    }

                    // Projects with card-like design
                    if (_content.Projects != null && _content.Projects.Count > 0)
                    {
                        column.Item().PaddingVertical(15).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.ConstantItem(20).Height(20).Width(20).Background(_styles.ColorScheme.GetSecondary());
                                r.ConstantItem(10);
                                r.RelativeItem().Text("NOTABLE PROJECTS").Style(_styles.HeadingStyle);
                            });
                            
                            c.Item().PaddingVertical(10).Element(container =>
                            {
                                container.Row(r =>
                                {
                                    foreach (var project in _content.Projects)
                                    {
                                        r.RelativeItem().Padding(5).Background(Colors.Grey.Lighten5).Border(0.5f)
                                            .BorderColor(_styles.GetDividerColor()).Column(pc =>
                                        {
                                            pc.Item().Background(_styles.ColorScheme.GetSecondary()).Padding(5)
                                                .Text(project.Title).Style(TextStyle.Default.FontColor("white").SemiBold());
                                            
                                            pc.Item().Padding(8).Column(desc =>
                                            {
                                                foreach (var bullet in project.Description)
                                                {
                                                    desc.Item().Text($"• {bullet}").Style(_styles.BulletPointStyle);
                                                    desc.Item().PaddingVertical(2);
                                                }
                                            });
                                        });
                                        
                                        r.ConstantItem(10);
                                    }
                                });
                            });
                        });
                    }

                    // Skills with visual skill bars
                    column.Item().PaddingVertical(15).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.ConstantItem(20).Height(20).Width(20).Background(_styles.ColorScheme.GetSecondary());
                            r.ConstantItem(10);
                            r.RelativeItem().Text("SKILLS & EXPERTISE").Style(_styles.HeadingStyle);
                        });
                        
                        c.Item().PaddingTop(10).Element(skillsContainer =>
                        {
                            skillsContainer.Row(r =>
                            {
                                r.RelativeItem().Column(col =>
                                {
                                    for (int i = 0; i < Math.Min(_content.Skills.Count, 5); i++)
                                    {
                                        col.Item().PaddingVertical(3).Text(_content.Skills[i]).Style(_styles.BulletPointStyle);
                                    }
                                });
                                
                                if (_content.Skills.Count > 5)
                                {
                                    r.RelativeItem().Column(col =>
                                    {
                                        for (int i = 5; i < _content.Skills.Count; i++)
                                        {
                                            col.Item().PaddingVertical(3).Text(_content.Skills[i]).Style(_styles.BulletPointStyle);
                                        }
                                    });
                                }
                            });
                        });
                    });

                    // Education with card design
                    column.Item().PaddingVertical(15).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.ConstantItem(20).Height(20).Width(20).Background(_styles.ColorScheme.GetSecondary());
                            r.ConstantItem(10);
                            r.RelativeItem().Text("EDUCATION").Style(_styles.HeadingStyle);
                        });
                        
                        c.Item().PaddingTop(10).Background(Colors.Grey.Lighten5).Border(0.5f)
                            .BorderColor(_styles.GetDividerColor()).Padding(15).Column(ec =>
                        {
                            ec.Item().Text(_content.Education.Degree).Style(_styles.SubheadingStyle);
                            ec.Item().Text(_content.Education.University).Style(_styles.NormalTextStyle.SemiBold());
                            ec.Item().Text(_content.Education.GradYear).Style(_styles.NormalTextStyle.Italic());
                            
                            if (_content.Education.Highlights != null && _content.Education.Highlights.Count > 0)
                            {
                                ec.Item().PaddingTop(5);
                                foreach (var highlight in _content.Education.Highlights)
                                {
                                    ec.Item().Text($"▹ {highlight}").Style(_styles.BulletPointStyle);
                                }
                            }
                        });
                    });
                });

                page.Footer().Height(30).Background(_styles.ColorScheme.GetPrimary()).Element(container =>
                {
                    container.Row(row =>
                    {
                        row.RelativeItem().AlignMiddle().AlignCenter().Text($"Generated on {DateTime.Now:MMMM dd, yyyy}")
                            .Style(TextStyle.Default.FontSize(8).FontColor("white"));
                    });
                });
            });
        }
    }
}
