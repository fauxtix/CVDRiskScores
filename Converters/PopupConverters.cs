using System.Globalization;

namespace CVDRiskScores.Converters
{
    // Detect boolean-like strings ("Sim"/"Não", "True"/"False", etc.)
    public class IsBooleanStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString()?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(s)) return false;
            return s == "sim" || s == "s" || s == "true" || s == "yes" || s == "y"
                || s == "não" || s == "nao" || s == "n" || s == "false" || s == "no";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    // Convert boolean-like string to glyph
    public class BooleanToGlyphConverter : IValueConverter
    {
        const string TrueGlyph = "✔";
        const string FalseGlyph = "✖";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString()?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s == "sim" || s == "s" || s == "true" || s == "yes" || s == "y") return TrueGlyph;
            if (s == "não" || s == "nao" || s == "n" || s == "false" || s == "no") return FalseGlyph;
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    // Color for boolean glyph (returns Color)
    public class BooleanGlyphColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString()?.Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(s)) return Colors.Transparent;

            if (s == "sim" || s == "s" || s == "true" || s == "yes" || s == "y")
            {
                if (Application.Current?.Resources?.TryGetValue("Primary", out var primObj) == true && primObj is Color primColor) return primColor;
                return Colors.DarkGreen;
            }

            if (s == "não" || s == "nao" || s == "n" || s == "false" || s == "no")
            {
                if (Application.Current?.Resources?.TryGetValue("Gray500", out var gobj) == true && gobj is Color grayColor) return grayColor;
                return Colors.Gray;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    // Highlight color for keys related to risk/pontuação
    public class KeyHighlightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var key = value?.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(key)) return Colors.Black;

            var lower = key.ToLowerInvariant();
            if (lower.Contains("risk") || lower.Contains("risco") || lower.Contains("pontua") || lower.Contains("pontuação") || lower.Contains("pontuaç"))
            {
                if (Application.Current?.Resources?.TryGetValue("Primary", out var primObj) == true && primObj is Color primColor) return primColor;
                return Color.FromArgb("#512BD4");
            }

            if (Application.Current?.Resources?.TryGetValue("Black", out var blackObj) == true && blackObj is Color blackColor) return blackColor;
            return Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    // Converts "[COLOR:#rrggbb]" or "#rrggbb" to SolidColorBrush for Ellipse.Fill
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(s)) return new SolidColorBrush(Colors.Transparent);

            if (s.StartsWith("[COLOR:", StringComparison.OrdinalIgnoreCase) && s.EndsWith("]"))
            {
                s = s.Substring(7, s.Length - 8).Trim();
            }

            if (!s.StartsWith("#"))
                s = s.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ? "#" + s.Substring(2) : "#" + s;

            try
            {
                var color = Color.FromArgb(s);
                return new SolidColorBrush(color);
            }
            catch
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }

    // Detects color marker strings so UI shows swatch instead of hex text
    public class IsColorMarkerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value?.ToString()?.Trim();
            if (string.IsNullOrEmpty(s)) return false;
            if (s.StartsWith("[COLOR:", StringComparison.OrdinalIgnoreCase) && s.EndsWith("]")) return true;
            if (s.StartsWith("#") && (s.Length == 7 || s.Length == 9)) return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}