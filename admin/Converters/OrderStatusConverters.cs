using System.Globalization;
using Shared.Models.Order;

namespace admin.Converters
{
    public class OrderStatusToNextActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status switch
                {
                    OrderStatus.Placed => "Start Preparing",
                    OrderStatus.Preparing => "Mark as Ready",
                    OrderStatus.Ready => "Mark as Served",
                    OrderStatus.Served => "Complete Order",
                    _ => "Update Status"
                };
            }
            return "Update Status";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class OrderStatusToCanUpdateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is OrderStatus status)
            {
                return status != OrderStatus.Completed && status != OrderStatus.Cancelled;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class BoolToFilterTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isDateFilter && parameter is string options)
            {
                var parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return isDateFilter ? parts[1] : parts[0];
                }
            }
            
            return "Toggle Filter";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}