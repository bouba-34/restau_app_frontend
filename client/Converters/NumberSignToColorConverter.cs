using System.Globalization;

namespace Client.Converters
{
    public class NumberSignToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue >= 0 
                    ? Color.FromArgb("#4CAF50") // Green for positive
                    : Color.FromArgb("#F44336"); // Red for negative
            }
            else if (value is int intValue)
            {
                return intValue >= 0 
                    ? Color.FromArgb("#4CAF50") // Green for positive
                    : Color.FromArgb("#F44336"); // Red for negative
            }
            else if (value is double doubleValue)
            {
                return doubleValue >= 0 
                    ? Color.FromArgb("#4CAF50") // Green for positive
                    : Color.FromArgb("#F44336"); // Red for negative
            }
            else if (value is float floatValue)
            {
                return floatValue >= 0 
                    ? Color.FromArgb("#4CAF50") // Green for positive
                    : Color.FromArgb("#F44336"); // Red for negative
            }
            
            return Color.FromArgb("#757575"); // Default gray
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}