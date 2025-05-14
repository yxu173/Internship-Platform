using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Application.Features.ResumeGeneration;
using System;
using System.Drawing;

namespace Infrastructure.Pdf.Templates
{
    public class TwoColumnResumeTemplate : IResumeTemplate
    {
        private readonly ResumeContent _content;
        private readonly ResumeStyles _styles;

        public TwoColumnResumeTemplate(ResumeContent content, ResumeStyles styles)
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

                page.Content().Row(row =>
                {
                    // Left sidebar - 1/3 width
                    row.RelativeItem(1).Background(_styles.ColorScheme.GetPrimary()).Padding(20).Column(leftColumn =>
                    {
                        // Name and contact in sidebar 
                        leftColumn.Item().AlignCenter().Text(_content.Title.Split('-')[0].Trim())
                            .Style(TextStyle.Default.FontSize(18).FontColor("white").FontFamily(_styles.HeaderFont).Bold());
                        
                        leftColumn.Item().PaddingVertical(20);
                        
                        // Contact Info
                        leftColumn.Item().Text("CONTACT")
                            .Style(TextStyle.Default.FontSize(14).FontColor("white").FontFamily(_styles.HeaderFont).SemiBold());
                        
                        leftColumn.Item().PaddingVertical(5).LineHorizontal(1).LineColor("white");
                        
                        leftColumn.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Email: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                            text.Span("student@example.com").Style(TextStyle.Default.FontColor("white").Italic());
                        });
                        
                        leftColumn.Item().PaddingTop(5).Text(text =>
                        {
                            text.Span("Phone: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                            text.Span(_content.PhoneNumber).Style(TextStyle.Default.FontColor("white").Italic());
                        });
                        
                        if (!string.IsNullOrEmpty(_content.Location))
                        {
                            leftColumn.Item().PaddingTop(5).Text(text =>
                            {
                                text.Span("Location: ").Style(TextStyle.Default.FontColor("white").SemiBold());
                                text.Span(_content.Location).Style(TextStyle.Default.FontColor("white").Italic());
                            });
                        }
                        
                        leftColumn.Item().PaddingVertical(20);
                        
                        // Skills section in sidebar
                        leftColumn.Item().Text("SKILLS")
                            .Style(TextStyle.Default.FontSize(14).FontColor("white").FontFamily(_styles.HeaderFont).SemiBold());
                        
                        leftColumn.Item().PaddingVertical(5).LineHorizontal(1).LineColor("white");
                        
                        foreach (var skill in _content.Skills)
                        {
                            leftColumn.Item().PaddingTop(3).Text($"• {skill}")
                                .Style(TextStyle.Default.FontColor("white").FontSize(9));
                        }
                        
                        leftColumn.Item().PaddingVertical(20);
                        
                        // Education in sidebar
                        leftColumn.Item().Text("EDUCATION")
                            .Style(TextStyle.Default.FontSize(14).FontColor("white").FontFamily(_styles.HeaderFont).SemiBold());
                        
                        leftColumn.Item().PaddingVertical(5).LineHorizontal(1).LineColor("white");
                        
                        leftColumn.Item().PaddingTop(5).Text(_content.Education.Degree)
                            .Style(TextStyle.Default.FontColor("white").SemiBold());
                        
                        leftColumn.Item().Text(_content.Education.University)
                            .Style(TextStyle.Default.FontColor("white").FontSize(9));
                        
                        leftColumn.Item().Text(_content.Education.GradYear)
                            .Style(TextStyle.Default.FontColor("white").FontSize(9).Italic());
                        
                        if (_content.Education.Highlights != null && _content.Education.Highlights.Count > 0)
                        {
                            leftColumn.Item().PaddingVertical(5);
                            foreach (var highlight in _content.Education.Highlights)
                            {
                                leftColumn.Item().Text($"• {highlight}")
                                    .Style(TextStyle.Default.FontColor("white").FontSize(9));
                            }
                        }
                    });
                    
                    // Main content - 2/3 width
                    row.RelativeItem(2).Padding(30).Column(rightColumn =>
                    {
                        // Professional Summary
                        rightColumn.Item().Text(position => 
                        {
                            if (_content.Title.Contains("-"))
                            {
                                var parts = _content.Title.Split('-');
                                if (parts.Length > 1)
                                {
                                    position.Span(parts[1].Trim()).Style(_styles.SubheadingStyle.FontSize(16));
                                }
                            }
                        });
                        
                        rightColumn.Item().PaddingVertical(10);
                        
                        rightColumn.Item().Text("SUMMARY")
                            .Style(_styles.HeadingStyle);
                        
                        rightColumn.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                        
                        rightColumn.Item().PaddingTop(5).Text(_content.Summary)
                            .Style(_styles.NormalTextStyle);
                        
                        rightColumn.Item().PaddingVertical(15);
                        
                        // Experience
                        if (_content.Experience != null && _content.Experience.Count > 0)
                        {
                            rightColumn.Item().Text("EXPERIENCE")
                                .Style(_styles.HeadingStyle);
                            
                            rightColumn.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                            
                            rightColumn.Item().PaddingVertical(5);
                            
                            foreach (var exp in _content.Experience)
                            {
                                rightColumn.Item().Element(container =>
                                {
                                    container.Column(c =>
                                    {
                                        c.Item().Element(titleContainer =>
                                        {
                                            titleContainer.Row(r =>
                                            {
                                                r.RelativeItem().Text(exp.Role)
                                                    .Style(_styles.SubheadingStyle);
                                                    
                                                if (!string.IsNullOrEmpty(exp.Duration))
                                                {
                                                    r.ConstantItem(100).AlignRight().Text(exp.Duration)
                                                        .Style(TextStyle.Default.Italic().FontColor(_styles.ColorScheme.Text).FontSize(8));
                                                }
                                            });
                                        });
                                        
                                        c.Item().Text(exp.Company)
                                            .Style(_styles.NormalTextStyle.Italic());
                                            
                                        c.Item().PaddingVertical(3);
                                        
                                        foreach (var bullet in exp.Description)
                                        {
                                            c.Item().PaddingLeft(8).Text($"• {bullet}")
                                                .Style(_styles.BulletPointStyle);
                                        }
                                    });
                                });
                                
                                rightColumn.Item().PaddingVertical(8);
                            }
                        }
                        
                        // Projects
                        if (_content.Projects != null && _content.Projects.Count > 0)
                        {
                            rightColumn.Item().Text("PROJECTS")
                                .Style(_styles.HeadingStyle);
                            
                            rightColumn.Item().PaddingBottom(5).LineHorizontal(0.5f).LineColor(_styles.GetDividerColor());
                            
                            rightColumn.Item().PaddingVertical(5);
                            
                            foreach (var project in _content.Projects)
                            {
                                rightColumn.Item().Element(container =>
                                {
                                    container.Column(c =>
                                    {
                                        c.Item().Text(project.Title)
                                            .Style(_styles.SubheadingStyle);
                                            
                                        c.Item().PaddingVertical(3);
                                        
                                        foreach (var bullet in project.Description)
                                        {
                                            c.Item().PaddingLeft(8).Text($"• {bullet}")
                                                .Style(_styles.BulletPointStyle);
                                        }
                                    });
                                });
                                
                                rightColumn.Item().PaddingVertical(5);
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
