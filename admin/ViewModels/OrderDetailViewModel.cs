using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    [QueryProperty(nameof(OrderId), "OrderId")]
    public partial class OrderDetailViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string orderId;

        [ObservableProperty]
        private Order order;

        [ObservableProperty]
        private ObservableCollection<OrderItem> orderItems = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private OrderStatus nextStatus;

        [ObservableProperty]
        private bool canUpdateStatus;

        [ObservableProperty]
        private string nextStatusButtonText;

        [ObservableProperty]
        private bool canCancelOrder;

        public OrderDetailViewModel(
            IOrderService orderService,
            INotificationService notificationService)
        {
            Title = "Order Details";
            _orderService = orderService;
            _notificationService = notificationService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await LoadOrderAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await LoadOrderAsync();
            IsRefreshing = false;
        }

        partial void OnOrderIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value) && IsInitialized)
            {
                LoadOrderAsync().ConfigureAwait(false);
            }
        }

        private async Task LoadOrderAsync()
        {
            if (string.IsNullOrEmpty(OrderId))
                return;

            try
            {
                IsLoading = true;
                ClearError();

                Order = await _orderService.GetOrderByIdAsync(OrderId);
                
                if (Order != null)
                {
                    OrderItems.Clear();
                    foreach (var item in Order.Items)
                    {
                        OrderItems.Add(item);
                    }

                    // Set next status and button text
                    nextStatus = GetNextOrderStatus(Order.Status);
                    NextStatusButtonText = GetStatusButtonText(Order.Status);
                    CanUpdateStatus = Order.Status != OrderStatus.Completed && Order.Status != OrderStatus.Cancelled;
                    CanCancelOrder = Order.Status != OrderStatus.Completed && Order.Status != OrderStatus.Cancelled;
                    
                    Title = $"Order #{Order.Id}";
                    Subtitle = $"{Order.CustomerName} - {Order.CreatedAt:MM/dd/yyyy HH:mm}";
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load order: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private OrderStatus GetNextOrderStatus(OrderStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Placed => OrderStatus.Preparing,
                OrderStatus.Preparing => OrderStatus.Ready,
                OrderStatus.Ready => OrderStatus.Served,
                OrderStatus.Served => OrderStatus.Completed,
                _ => currentStatus
            };
        }

        private string GetStatusButtonText(OrderStatus currentStatus)
        {
            return currentStatus switch
            {
                OrderStatus.Placed => "Start Preparing",
                OrderStatus.Preparing => "Mark as Ready",
                OrderStatus.Ready => "Mark as Served",
                OrderStatus.Served => "Complete Order",
                _ => "Update Status"
            };
        }

        [RelayCommand]
        private async Task UpdateStatusAsync()
        {
            if (Order == null)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _orderService.UpdateOrderStatusAsync(OrderId, NextStatus);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Order.CustomerId,
                        $"Order #{Order.Id} Status Update",
                        $"Your order is now {NextStatus}.");

                    // Reload order to get latest status
                    await LoadOrderAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update order status: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelOrderAsync()
        {
            if (Order == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Cancel Order",
                "Are you sure you want to cancel this order?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _orderService.CancelOrderAsync(OrderId);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Order.CustomerId,
                        $"Order #{Order.Id} Cancelled",
                        "Your order has been cancelled.");

                    // Reload order to get latest status
                    await LoadOrderAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to cancel order: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ProcessPaymentAsync()
        {
            if (Order == null)
                return;

            // This would typically open a payment processing page or dialog
            // For this example, we'll just mark the payment as complete

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _orderService.ProcessPaymentAsync(OrderId, "CreditCard", PaymentStatus.Paid);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Order.CustomerId,
                        $"Order #{Order.Id} Payment Processed",
                        "Your payment has been processed successfully.");

                    // Reload order to get latest status
                    await LoadOrderAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to process payment: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}