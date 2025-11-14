using CVDRiskScores.Enums;
using System.Globalization;

namespace CVDRiskScores.Converters
{
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            var genderValue = (Genero)value;
            // Adjust this condition based on your logic to determine visibility
            if (genderValue == Genero.Male)
            {
                return Visibility.Visible;
            }
            return value.ToString() == "Category";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
