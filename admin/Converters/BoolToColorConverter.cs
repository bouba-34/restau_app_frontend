using System.Globalization;

namespace admin.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string colors)
            {
                string[] colorOptions = colors.Split('|');
                if (colorOptions.Length >= 2)
                {
                    string colorName = boolValue ? colorOptions[0] : colorOptions[1];
                    
                    // Try to get color from resources
                    if (Application.Current.Resources.TryGetValue(colorName, out object resourceColor) && resourceColor is Color color)
                    {
                        return color;
                    }
                    
                    // Try to parse color directly
                    return Color.Parse(colorName);
                }
            }
            
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string options)
            {
                string[] textOptions = options.Split('|');
                if (textOptions.Length >= 2)
                {
                    return boolValue ? textOptions[0] : textOptions[1];
                }
            }
            
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}