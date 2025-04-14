using Shared.Models.Order;
using System.Globalization;

namespace Client.Converters
{
    public class OrderStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Placed => Color.FromArgb("#03A9F4"),     // Blue
                    OrderStatus.Preparing => Color.FromArgb("#FF9800"),  // Orange
                    OrderStatus.Ready => Color.FromArgb("#8BC34A"),      // Light Green
                    OrderStatus.Served => Color.FromArgb("#4CAF50"),     // Green
                    OrderStatus.Completed => Color.FromArgb("#009688"),  // Teal
                    OrderStatus.Cancelled => Color.FromArgb("#F44336"), // Red
                    _ => Color.FromArgb("#757575")                      // Grey
                };
            }
            
            return Color.FromArgb("#757575"); // Default gray
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}