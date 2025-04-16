using System.Globalization;
using Shared.Models.Order;
using Shared.Models.Reservation;

namespace admin.Converters
{
    public class OrderStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Placed => Application.Current.Resources["PlacedColor"],
                    OrderStatus.Preparing => Application.Current.Resources["PreparingColor"],
                    OrderStatus.Ready => Application.Current.Resources["ReadyColor"],
                    OrderStatus.Served => Application.Current.Resources["ServedColor"],
                    OrderStatus.Completed => Application.Current.Resources["CompletedColor"],
                    OrderStatus.Cancelled => Application.Current.Resources["CancelledColor"],
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PaymentStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PaymentStatus status)
            {
                return status switch
                {
                    PaymentStatus.Paid => Application.Current.Resources["PaidColor"],
                    PaymentStatus.Pending => Application.Current.Resources["PendingColor"],
                    PaymentStatus.Failed => Application.Current.Resources["FailedColor"],
                    PaymentStatus.Refunded => Application.Current.Resources["RefundedColor"],
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ReservationStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReservationStatus status)
            {
                return status switch
                {
                    ReservationStatus.Pending => Application.Current.Resources["ReservationPendingColor"],
                    ReservationStatus.Confirmed => Application.Current.Resources["ConfirmedColor"],
                    ReservationStatus.Completed => Application.Current.Resources["CompletedColor"],
                    ReservationStatus.Cancelled => Application.Current.Resources["CancelledColor"],
                    ReservationStatus.NoShow => Application.Current.Resources["NoShowColor"],
                    _ => Colors.Gray
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}