using System.Collections;
using System.Globalization;

namespace Client.Converters
{
    public class CountToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double itemHeight = 30; // Default height per item
            
            if (parameter != null && double.TryParse(parameter.ToString(), out double height))
            {
                itemHeight = height;
            }
            
            if (value is int count)
            {
                return count * itemHeight;
            }
            
            if (value is ICollection collection)
            {
                return collection.Count * itemHeight;
            }
            
            return 0;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}