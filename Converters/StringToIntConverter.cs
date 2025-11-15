using System.Globalization;

namespace CVDRiskScores.Converters
{
    public class StringToIntConverter : IValueConverter
    {
        // value will be int? (nullable) from the VM
        public object? Convert(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is int i)
                return i.ToString(culture);
            return string.Empty; // show placeholder when null
        }

        // return int? (nullable) so VM property can stay null when entry is empty / invalid
        public object? ConvertBack(object? value, Type? targetType, object? parameter, CultureInfo? culture)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s) && int.TryParse(s, NumberStyles.Integer, culture, out var result))
                return result;

            return null;
        }
    }
}