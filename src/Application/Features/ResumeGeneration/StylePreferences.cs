using System.Collections.Generic;

namespace Application.Features.ResumeGeneration;

public class StylePreferences
{
    // Template selection
    public string TemplateName { get; set; } // "Classic", "Modern", "Creative", "Minimalist", "TwoColumn"
    
    // Theme-based styling
    public string ThemeName { get; set; } // "Professional", "Modern", "Creative", etc.
    
    // Font preferences
    public string MainFont { get; set; } // Font for body text
    public string HeaderFont { get; set; } // Font for headers and titles
    
    // Custom color scheme (if not using a theme)
    public CustomColorScheme CustomColors { get; set; }
}

public class CustomColorScheme
{
    public string PrimaryColor { get; set; } // Main title color
    public string SecondaryColor { get; set; } // Section headings color
    public string TertiaryColor { get; set; } // Subheadings color
    public string TextColor { get; set; } // Normal text color
    public string DividerColor { get; set; } // Color of horizontal dividers
}
