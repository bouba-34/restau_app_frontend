using Shared.Models.Reservation;
using System.Globalization;

namespace Client.Converters
{
    public class ReservationStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReservationStatus status)
            {
                return status switch
                {
                    ReservationStatus.Pending => Colors.Orange,
                    ReservationStatus.Confirmed => Colors.Green,
                    ReservationStatus.Completed => Colors.Blue,
                    ReservationStatus.Cancelled => Colors.Red,
                    ReservationStatus.NoShow => Colors.Red,
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