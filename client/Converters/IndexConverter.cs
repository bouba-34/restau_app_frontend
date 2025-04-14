using System.Collections;
using System.Globalization;

namespace Client.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return 0;
                
            var collectionView = System.Maui.VisualStateManager.GetVisualStateManager(value);
            var index = -1;
            
            if (collectionView is CollectionView collection)
            {
                if (collection.ItemsSource is IList items)
                {
                    index = items.IndexOf(value);
                }
            }
            
            // Add offset if parameter exists
            if (parameter != null && int.TryParse(parameter.ToString(), out var offset))
            {
                index += offset;
            }
            
            return index;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}