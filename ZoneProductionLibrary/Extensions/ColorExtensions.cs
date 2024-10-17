namespace ZoneProductionLibrary.Extensions
{
    public static class ColorExtensions
    {
        public static List<string> ColorStrings { get; } =
            ["#e60049", "#0bb4ff", "#50e991", "#e6d800", "#9b19f5", "#ffa300", "#dc0ab4", "#b3d4ff", "#00bfa0"];

        public static readonly List<string> AltColorStrings =
        [
            "#a61c00", "#cc0000", "#e69138", "#f1c232", "#6aa84f", "#45818e", "#3c78d8", "#3d85c6", "#674ea7", "#a64d79",
            "#cc4125", "#e06666", "#f6b26b", "#ffd966", "#93c47d", "#76a5af", "#6d9eeb", "#6fa8dc", "#8e7cc3", "#c27ba0",
            "#dd7e6b", "#ea9999", "#f9cb9c", "#ffe599", "#b6d7a8", "#a2c4c9", "#a4c2f4", "#9fc5e8", "#b4a7d6", "#d5a6bd"
        ];
        
        public static string ToChartColor(this RedFlagIssue issue)
            => AltColorStrings.ElementAt((int)issue % AltColorStrings.Count);
        
        public static string ToChartColor(this CardAreaOfOrigin area)
            => AltColorStrings.ElementAt((int)area % AltColorStrings.Count);
        
        public static string ToHex(this System.Drawing.Color c)
        {
            var s = $"#{c.R:X2}{c.G:X2}{c.B:X2}";

            return s;
        }
    }
}