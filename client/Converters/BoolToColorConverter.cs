using System.Globalization;

namespace Client.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string trueColor = "#00C853"; // Default green
            string falseColor = "#616161"; // Default gray
            
            // Parse parameter
            if (parameter is string colorParams)
            {
                var colors = colorParams.Split(',');
                if (colors.Length >= 2)
                {
                    trueColor = colors[0];
                    falseColor = colors[1];
                }
            }
            
            return value is bool boolValue && boolValue 
                ? Color.FromArgb(trueColor) 
                : Color.FromArgb(falseColor);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}