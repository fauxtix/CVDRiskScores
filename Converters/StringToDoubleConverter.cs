using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace CVDRiskScores.Converters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            var ci = culture ?? CultureInfo.CurrentCulture;
            if (value is double d)
                return d.ToString("G", ci);

            if (value == null)
                return string.Empty;

            // try to handle boxed nullable double
            try
            {
                var s = System.Convert.ToDouble(value);
                return s.ToString("G", ci);
            }
            catch
            {
                return string.Empty;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
        {
            var ci = culture ?? CultureInfo.CurrentCulture;
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return null;

            if (double.TryParse(str, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, ci, out var v))
                return v;
            if (double.TryParse(str, System.Globalization.NumberStyles.Float | System.Globalization.NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out v))
                return v;

            return null;
        }
    }
}
