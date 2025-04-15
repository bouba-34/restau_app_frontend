using Shared.Models.Order;

namespace Client.Controls
{
    public partial class OrderStatusIndicator : ContentView
    {
        public static readonly BindableProperty OrderStatusProperty =
            BindableProperty.Create(nameof(OrderStatus), typeof(OrderStatus), typeof(OrderStatusIndicator), OrderStatus.Placed, propertyChanged: OnOrderStatusChanged);

        public static readonly BindableProperty EstimatedWaitTimeProperty =
            BindableProperty.Create(nameof(EstimatedWaitTime), typeof(int), typeof(OrderStatusIndicator), 0, propertyChanged: OnEstimatedWaitTimeChanged);

        public OrderStatus OrderStatus
        {
            get => (OrderStatus)GetValue(OrderStatusProperty);
            set => SetValue(OrderStatusProperty, value);
        }
        
        public int EstimatedWaitTime
        {
            get => (int)GetValue(EstimatedWaitTimeProperty);
            set => SetValue(EstimatedWaitTimeProperty, value);
        }

        public OrderStatusIndicator()
        {
            InitializeComponent();
        }

        private static void OnOrderStatusChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (OrderStatusIndicator)bindable;
            
            if (newValue is OrderStatus status)
            {
                control.StatusLabel.Text = GetStatusText(status);
                control.StatusIndicator.BackgroundColor = GetStatusColor(status);
                
                // Show estimated time for certain statuses
                control.EstimatedTimeLabel.IsVisible = status == OrderStatus.Placed || status == OrderStatus.Preparing;
            }
        }
        
        private static void OnEstimatedWaitTimeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (OrderStatusIndicator)bindable;
            
            if (newValue is int minutes && minutes > 0)
            {
                control.EstimatedTimeLabel.Text = $"Est. time: {minutes} min";
                control.EstimatedTimeLabel.IsVisible = true;
            }
            else
            {
                control.EstimatedTimeLabel.Text = string.Empty;
                control.EstimatedTimeLabel.IsVisible = false;
            }
        }
        
        private static string GetStatusText(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => "Order Placed",
                OrderStatus.Preparing => "Preparing",
                OrderStatus.Ready => "Ready",
                OrderStatus.Served => "Served",
                OrderStatus.Completed => "Completed",
                OrderStatus.Cancelled => "Cancelled",
                _ => status.ToString()
            };
        }
        
        private static Color GetStatusColor(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Placed => Colors.Orange,
                OrderStatus.Preparing => Colors.Blue,
                OrderStatus.Ready => Colors.Green,
                OrderStatus.Served => Colors.Purple,
                OrderStatus.Completed => Colors.Green,
                OrderStatus.Cancelled => Colors.Red,
                _ => Colors.Gray
            };
        }
    }
}