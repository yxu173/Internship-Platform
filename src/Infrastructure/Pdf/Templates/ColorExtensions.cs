using System;
using System.Globalization;

namespace Infrastructure.Pdf.Templates
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Converts a hex color string (e.g., "#RRGGBB" or "RRGGBB") to an RGB color suitable for QuestPDF
        /// </summary>
        public static string ToRgbColor(this string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return "#000000";
            
            // Remove # if present
            if (hexColor.StartsWith("#"))
                hexColor = hexColor.Substring(1);
                
            // Ensure it's a valid hex color
            if (hexColor.Length != 6)
                return "#000000";
                
            try
            {
                return hexColor;
            }
            catch
            {
                return "#000000";
            }
        }
    }
}
