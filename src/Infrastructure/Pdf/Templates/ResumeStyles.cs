using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace Infrastructure.Pdf.Templates
{
    public static class ResumeStyles
    {
        public static TextStyle TitleStyle => TextStyle
            .Default
            .FontSize(20)
            .SemiBold()
            .FontColor("#1a365d");

        public static TextStyle HeadingStyle => TextStyle
            .Default
            .FontSize(14)
            .SemiBold()
            .FontColor("#2c5282")
            .FontFamily("Arial");

        public static TextStyle SubheadingStyle => TextStyle
            .Default
            .FontSize(12)
            .SemiBold()
            .FontColor("#2d3748");

        public static TextStyle NormalTextStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor("#4a5568");

        public static TextStyle ContactInfoStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor("#4a5568")
            .Italic();

        public static TextStyle BulletPointStyle => TextStyle
            .Default
            .FontSize(10)
            .FontColor("#4a5568");
    }
}
