using System.Globalization;
using Shared.Models.Order;

namespace admin.Converters
{
    public class PaymentStatusIsNotPaidConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PaymentStatus status)
            {
                return status != PaymentStatus.Paid && status != PaymentStatus.Refunded;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}