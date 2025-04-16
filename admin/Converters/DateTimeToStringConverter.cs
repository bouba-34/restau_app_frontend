using System.Globalization;

namespace admin.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                string format = parameter as string ?? "MM/dd/yyyy HH:mm";
                return dateTime.ToString(format);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string dateString)
            {
                string format = parameter as string ?? "MM/dd/yyyy HH:mm";
                if (DateTime.TryParseExact(dateString, format, culture, DateTimeStyles.None, out DateTime result))
                {
                    return result;
                }
            }
            return DateTime.Now;
        }
    }
}