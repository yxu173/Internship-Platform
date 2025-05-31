using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using System;

namespace Infrastructure.Pdf.Templates
{
    public class ModernResumeTemplate : IResumeTemplate
    {
        private readonly ResumeContent _content;
        private readonly ResumeStyles _styles;

        public ModernResumeTemplate(ResumeContent content, ResumeStyles styles)
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
                
                // Name and title banner at the top with accent color
                page.Header().Height(100).Column(column =>
                {
                    column.Item().Height(60).Background(_styles.ColorScheme.GetPrimary()).Padding(20)
                        .AlignMiddle().Row(row =>
                    {
                        row.RelativeItem().AlignMiddle().Text(_content.Title)
                            .Style(TextStyle.Default.FontSize(22).FontColor("white").FontFamily(_styles.HeaderFont).Bold());
                    });
                    
                    // Contact information bar with secondary color
                    column.Item().Height(40).Background(_styles.ColorScheme.GetSecondary()).PaddingHorizontal(20)
                        .AlignMiddle().Row(row =>
                    {
                        row.RelativeItem().AlignMiddle().Text(text =>
                        {
                            text.Span("Email: ").FontColor("white").SemiBold();
                            text.Span("student@example.com").FontColor("white");
                        });
                        
                        row.RelativeItem().AlignMiddle().AlignCenter().Text(text =>
                        {
                            text.Span("Phone: ").FontColor("white").SemiBold();
                            text.Span(_content.PhoneNumber).FontColor("white");
                        });
                        
                        if (!string.IsNullOrEmpty(_content.Location))
                        {
                            row.RelativeItem().AlignMiddle().AlignRight().Text(text =>
                            {
                                text.Span("Location: ").FontColor("white").SemiBold();
                                text.Span(_content.Location).FontColor("white");
                            });
                        }
                    });
                });

                // Main content with modern design
                page.Content().Padding(20).Column(column =>
                {
                    // Summary with modern heading
                    column.Item().PaddingBottom(15).Column(c =>
                    {
                        c.Item().PaddingBottom(5).Text("PROFESSIONAL PROFILE")
                            .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                        
                        c.Item().Border(1).BorderColor(_styles.ColorScheme.GetPrimary());
                        
                        c.Item().PaddingTop(10).Text(_content.Summary)
                            .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.Text));
                    });

                    // Experience with modern sections
                    if (_content.Experience != null && _content.Experience.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().PaddingBottom(5).Text("WORK EXPERIENCE")
                                .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                                
                            c.Item().Border(1).BorderColor(_styles.ColorScheme.GetPrimary());
                            
                            foreach (var exp in _content.Experience)
                            {
                                c.Item().PaddingTop(10).Padding(10).BorderLeft(3).BorderColor(_styles.ColorScheme.GetSecondary()).Column(ec =>
                                {
                                    ec.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text(exp.Role)
                                            .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).SemiBold());
                                            
                                        r.ConstantItem(100).AlignRight().Text(exp.Duration)
                                            .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()).Italic());
                                    });
                                    
                                    ec.Item().Text(exp.Company)
                                        .Style(TextStyle.Default.FontSize(11).FontColor(_styles.ColorScheme.GetText()));
                                        
                                    ec.Item().PaddingTop(5);
                                    
                                    foreach (var bullet in exp.Description)
                                    {
                                        ec.Item().Text($"• {bullet}")
                                            .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                    }
                                });
                            }
                        });
                    }

                    // Skills in modern 3-column grid
                    column.Item().PaddingVertical(10).Column(c =>
                    {
                        c.Item().PaddingBottom(5).Text("PROFESSIONAL SKILLS")
                            .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                            
                        c.Item().Border(1).BorderColor(_styles.ColorScheme.GetPrimary());
                        
                        c.Item().PaddingTop(10).Element(container =>
                        {
                            var skillCount = _content.Skills.Count;
                            var columns = 3; // Modern 3-column layout
                            var skillsPerColumn = (int)Math.Ceiling(skillCount / (double)columns);
                            
                            container.Row(row => 
                            {
                                for (int col = 0; col < columns; col++)
                                {
                                    if (col * skillsPerColumn < skillCount)
                                    {
                                        row.RelativeItem().PaddingHorizontal(5).Column(colContainer =>
                                        {
                                            for (int i = col * skillsPerColumn; i < Math.Min((col + 1) * skillsPerColumn, skillCount); i++)
                                            {
                                                if (i < _content.Skills.Count)
                                                {
                                                    colContainer.Item().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten3)
                                                        .Padding(5).Text(_content.Skills[i])
                                                        .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                                }
                                            }
                                        });
                                    }
                                }
                            });
                        });
                    });

                    // Projects in modern cards
                    if (_content.Projects != null && _content.Projects.Count > 0)
                    {
                        column.Item().PaddingVertical(10).Column(c =>
                        {
                            c.Item().PaddingBottom(5).Text("KEY PROJECTS")
                                .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                                
                            c.Item().Border(1).BorderColor(_styles.ColorScheme.GetPrimary());
                            
                            c.Item().PaddingTop(10).Row(r =>
                            {
                                foreach (var project in _content.Projects)
                                {
                                    r.RelativeItem().Border(0.5f).BorderColor(Colors.Grey.Lighten3)
                                        .Padding(10).Column(pc =>
                                    {
                                        pc.Item().BorderBottom(1).BorderColor(_styles.ColorScheme.Secondary).PaddingBottom(5)
                                            .Text(project.Title)
                                            .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.GetSecondary()).SemiBold());
                                            
                                        pc.Item().PaddingTop(5);
                                        
                                        foreach (var bullet in project.Description)
                                        {
                                            pc.Item().Text($"• {bullet}")
                                                .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                        }
                                    });
                                    
                                    r.ConstantItem(10);
                                }
                            });
                        });
                    }
                    
                    // Education with modern style
                    column.Item().PaddingVertical(10).Column(c =>
                    {
                        c.Item().PaddingBottom(5).Text("EDUCATION")
                            .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetPrimary()).FontFamily(_styles.HeaderFont).Bold());
                            
                        c.Item().Border(1).BorderColor(_styles.ColorScheme.GetPrimary());
                        
                        c.Item().PaddingTop(10).Row(r =>
                        {
                            r.RelativeItem(1).AlignMiddle().AlignCenter().Border(1).BorderColor(_styles.ColorScheme.GetSecondary())
                                .Height(90).Width(90).AlignCenter().AlignMiddle()
                                .Text(_content.Education.GradYear)
                                .Style(TextStyle.Default.FontSize(14).FontColor(_styles.ColorScheme.GetSecondary()).SemiBold());
                                
                            r.RelativeItem(3).PaddingLeft(20).Column(ec =>
                            {
                                ec.Item().Text(_content.Education.Degree)
                                    .Style(TextStyle.Default.FontSize(12).FontColor(_styles.ColorScheme.Secondary).SemiBold());
                                    
                                ec.Item().Text(_content.Education.University)
                                    .Style(TextStyle.Default.FontSize(11).FontColor(_styles.ColorScheme.Text));
                                    
                                if (_content.Education.Highlights != null && _content.Education.Highlights.Count > 0)
                                {
                                    ec.Item().PaddingTop(5);
                                    
                                    foreach (var highlight in _content.Education.Highlights)
                                    {
                                        ec.Item().Text($"• {highlight}")
                                            .Style(TextStyle.Default.FontSize(10).FontColor(_styles.ColorScheme.GetText()));
                                    }
                                }
                            });
                        });
                    });
                });

                page.Footer().Height(30).Background(_styles.ColorScheme.GetSecondary()).AlignCenter().AlignMiddle()
                    .Text($"Generated on {DateTime.Now:MMMM dd, yyyy}")
                    .Style(TextStyle.Default.FontSize(8).FontColor("white"));
            });
        }
    }
}
