using System.Globalization;

namespace CVDRiskScores.Converters
{
    public class StringToDateConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string s && DateTime.TryParse(s, out var date))
                return date;
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
                return dt.ToString("yyyy-MM-dd");
            return null;
        }
    }
}