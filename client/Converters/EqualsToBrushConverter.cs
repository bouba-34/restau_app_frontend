using System.Globalization;

namespace Client.Converters
{
    public class EqualsToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string trueColor = "#00C853"; // Default green
            string falseColor = "#EEEEEE"; // Default gray
            object compareValue = null;
            
            // Parse parameter
            if (parameter is string paramStr)
            {
                var parts = paramStr.Split(',');
                if (parts.Length >= 1)
                {
                    if (value is decimal && decimal.TryParse(parts[0], out var decimalValue))
                    {
                        compareValue = decimalValue;
                    }
                    else if (value is int && int.TryParse(parts[0], out var intValue))
                    {
                        compareValue = intValue;
                    }
                    else
                    {
                        compareValue = parts[0];
                    }
                }
                
                if (parts.Length >= 3)
                {
                    trueColor = parts[1];
                    falseColor = parts[2];
                }
            }
            
            return value?.Equals(compareValue) == true
                ? Color.FromArgb(trueColor)
                : Color.FromArgb(falseColor);
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}