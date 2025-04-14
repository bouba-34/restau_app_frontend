using System.Collections;
using System.Globalization;

namespace Client.Converters
{
    public class CountToInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count <= 0;
            }
            
            if (value is ICollection collection)
            {
                return collection.Count <= 0;
            }
            
            return true;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}