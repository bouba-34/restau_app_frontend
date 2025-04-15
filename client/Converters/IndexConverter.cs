using System;
using System.Collections;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Client.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return -1;

            var parentBindingContext = parameter as IEnumerable;

            if (parentBindingContext is IList list)
            {
                int index = list.IndexOf(value);
                return index >= 0 ? index : -1;
            }

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}