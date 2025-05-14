using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.Drawing;

namespace Infrastructure.Pdf.Templates
{
    public class ResumeStyles
    {
        // Predefined themes
        public static readonly Dictionary<string, ResumeColorScheme> ColorSchemes = new()
        {
            ["Professional"] = new ResumeColorScheme
            {
                Primary = "#1a365d",
                Secondary = "#2c5282",
                Tertiary = "#2d3748",
                Text = "#4a5568",
                Divider = "#CBD5E0"
            },
            ["Modern"] = new ResumeColorScheme
            {
                Primary = "#1a202c",
                Secondary = "#2d3748",
                Tertiary = "#4a5568",
                Text = "#718096",
                Divider = "#E2E8F0"
            },
            ["Creative"] = new ResumeColorScheme
            {
                Primary = "#553C9A",
                Secondary = "#6B46C1",
                Tertiary = "#805AD5",
                Text = "#4A5568",
                Divider = "#D6BCFA"
            },
            ["Bold"] = new ResumeColorScheme
            {
                Primary = "#c53030",
                Secondary = "#e53e3e",
                Tertiary = "#f56565",
                Text = "#4a5568",
                Divider = "#FED7D7"
            },
            ["Elegant"] = new ResumeColorScheme
            {
                Primary = "#2C7A7B",
                Secondary = "#319795",
                Tertiary = "#38B2AC",
                Text = "#4A5568",
                Divider = "#B2F5EA"
            },
            ["Corporate"] = new ResumeColorScheme
            {
                Primary = "#1A365D",
                Secondary = "#2A4365",
                Tertiary = "#2C5282",
                Text = "#4A5568",
                Divider = "#BEE3F8"
            },
            ["Minimalist"] = new ResumeColorScheme
            {
                Primary = "#1A202C",
                Secondary = "#2D3748",
                Tertiary = "#4A5568",
                Text = "#718096",
                Divider = "#E2E8F0"
            },
        };

        // List of supported font families
        public static readonly List<string> SupportedFonts = new()
        {
            "Arial",
            "Helvetica",
            "Times New Roman",
            "Calibri",
            "Georgia",
            "Garamond",
            "Verdana",
            "Roboto",
            "Tahoma",
            "Trebuchet MS",
        };

        public ResumeColorScheme ColorScheme { get; private set; }
        public string MainFont { get; private set; }
        public string HeaderFont { get; private set; }

        public ResumeStyles(string themeName = "Professional", string mainFont = "Arial", string headerFont = null)
        {
            // Set color scheme from predefined themes or use Professional as default
            ColorScheme = ColorSchemes.TryGetValue(themeName, out var scheme) ? 
                scheme : 
                ColorSchemes["Professional"];
            
            // Validate and set fonts
            MainFont = SupportedFonts.Contains(mainFont) ? mainFont : "Arial";
            HeaderFont = SupportedFonts.Contains(headerFont ?? mainFont) ? (headerFont ?? mainFont) : MainFont;
        }

        // Custom constructor for completely custom color scheme
        public ResumeStyles(ResumeColorScheme customColors, string mainFont = "Arial", string headerFont = null)
        {
            ColorScheme = customColors;
            MainFont = SupportedFonts.Contains(mainFont) ? mainFont : "Arial";
            HeaderFont = SupportedFonts.Contains(headerFont ?? mainFont) ? (headerFont ?? mainFont) : MainFont;
        }

        public TextStyle TitleStyle => TextStyle
            .Default
            .FontSize(20)
            .SemiBold()
            .FontColor(ColorScheme.Primary)
            .FontFamily(HeaderFont);

        public TextStyle HeadingStyle => TextStyle
            .Default
            .FontSize(14)
            .SemiBold()
            .FontColor(ColorScheme.Secondary)
            .FontFamily(HeaderFont);

        public TextStyle SubheadingStyle => TextStyle
            .Default
            .FontSize(12)
            .SemiBold()
            .FontColor(ColorScheme.Tertiary)
            .FontFamily(HeaderFont);

        public TextStyle NormalTextStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor(ColorScheme.Text)
            .FontFamily(MainFont);

        public TextStyle ContactInfoStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor(ColorScheme.Text)
            .FontFamily(MainFont)
            .Italic();

        public TextStyle BulletPointStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor(ColorScheme.Text)
            .FontFamily(MainFont);

        public string GetDividerColor() => ColorScheme.GetDivider();
    }

    public class ResumeColorScheme
    {
        public string Primary { get; set; } // Main title color
        public string Secondary { get; set; } // Section headings color
        public string Tertiary { get; set; } // Subheadings color
        public string Text { get; set; } // Normal text color
        public string Divider { get; set; } // Color of horizontal dividers
        
        // Methods to convert hex strings to QuestPDF-compatible colors
        public string GetPrimary() => Primary;
        public string GetSecondary() => Secondary;
        public string GetTertiary() => Tertiary;
        public string GetText() => Text;
        public string GetDivider() => Divider;
    }
}
