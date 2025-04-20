using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;
using Client.Constants;
using Plugin.LocalNotification;
using Shared.Models.Order;
using Shared.Services.Interfaces;

namespace Client.ViewModels.Order
{
    public partial class OrderDetailViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IOrderService _orderService;
        private readonly ISignalRService _signalRService;
        private string _orderId;
        
        [ObservableProperty]
        private Shared.Models.Order.Order _order;
        
        [ObservableProperty]
        private ObservableCollection<OrderItem> _orderItems;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _canCancel;
        
        [ObservableProperty]
        private int _estimatedWaitTime;
        
        [ObservableProperty]
        private bool _shouldShowEstimatedTime;
        
        [ObservableProperty]
        private string _paymentStatus;
        
        public OrderDetailViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IOrderService orderService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _orderService = orderService;
            _signalRService = signalRService;
            
            Title = "Order Details";
            OrderItems = new ObservableCollection<OrderItem>();
            
            // Subscribe to SignalR events
            _signalRService.OrderStatusChanged += OnOrderStatusChanged;
        }
        
        ~OrderDetailViewModel()
        {
            // Unsubscribe from SignalR events
            _signalRService.OrderStatusChanged -= OnOrderStatusChanged;
        }
        
        private void OnOrderStatusChanged(object sender, (string OrderId, OrderStatus Status) e)
        {
            if (e.OrderId == _orderId)
            {
                // Notification locale
                var request = new NotificationRequest
                {
                    NotificationId = new Random().Next(),
                    Title = "Order Update",
                    Description = $"Your order is now {e.Status}.",
                    //ReturningData = "OrderDetail", 
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = DateTime.Now.AddSeconds(1) 
                    }
                };

                LocalNotificationCenter.Current.Show(request);
                
                
                // Update order status
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadOrderAsync();
                });
            }
        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("OrderId", out var orderId))
            {
                _orderId = orderId.ToString();
                LoadOrderAsync().ConfigureAwait(false);
            }
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadOrderAsync();
        }
        
        private async Task LoadOrderAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    // Load order details
                    Order = await _orderService.GetOrderByIdAsync(_orderId);
                    
                    if (Order != null)
                    {
                        // Update title with order ID
                        Title = $"Order #{Order.Id.Substring(Math.Max(0, Order.Id.Length - 6))}";
                        
                        // Update items collection
                        OrderItems.Clear();
                        foreach (var item in Order.Items)
                        {
                            OrderItems.Add(item);
                        }
                        
                        // Set payment status
                        PaymentStatus = GetPaymentStatusText(Order.PaymentStatus);
                        
                        // Update estimated wait time if applicable
                        if (Order.Status == OrderStatus.Placed || Order.Status == OrderStatus.Preparing)
                        {
                            EstimatedWaitTime = await _orderService.GetEstimatedWaitTimeAsync(_orderId);
                            ShouldShowEstimatedTime = EstimatedWaitTime > 0;
                        }
                        else
                        {
                            ShouldShowEstimatedTime = false;
                        }
                        
                        // Determine if order can be cancelled
                        CanCancel = Order.Status == OrderStatus.Placed;
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load order details", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        [RelayCommand]
        private async Task CancelOrderAsync()
        {
            if (Order == null)
                return;
                
            bool confirm = await DialogService.DisplayAlertAsync(
                "Cancel Order", 
                Messages.ConfirmCancelOrder, 
                "Yes, Cancel", 
                "No");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    bool success = await _orderService.CancelOrderAsync(_orderId);
                    
                    if (success)
                    {
                        // Refresh order to show cancelled status
                        await LoadOrderAsync();
                        
                        // Show confirmation
                        await DialogService.DisplayToastAsync("Order cancelled successfully");
                        
                        // Navigate back to order history
                        await NavigationService.GoBackAsync();
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Error", "Failed to cancel order", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to cancel order", ex);
                }
            }, "Cancelling order...");
        }
        
        [RelayCommand]
        private async Task ReorderAsync()
        {
            if (Order == null || Order.Items.Count == 0)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Clear current cart
                    _orderService.ClearCart();
                    
                    // Add all items from this order to cart
                    foreach (var item in Order.Items)
                    {
                        var cartItem = new CartItem
                        {
                            MenuItemId = item.MenuItemId,
                            Name = item.MenuItemName,
                            ImageUrl = item.MenuItemImageUrl,
                            Price = item.UnitPrice,
                            Quantity = item.Quantity,
                            SpecialInstructions = item.SpecialInstructions,
                            Customizations = new List<string>(item.Customizations)
                        };
                        
                        _orderService.AddToCart(cartItem);
                    }
                    
                    // Copy special instructions if any
                    if (!string.IsNullOrEmpty(Order.SpecialInstructions))
                    {
                        _orderService.UpdateCartSpecialInstructions(Order.SpecialInstructions);
                    }
                    
                    // Copy tip amount
                    _orderService.UpdateCartTipAmount(Order.TipAmount);
                    
                    // Copy order type
                    _orderService.UpdateCartOrderType(Order.Type);
                    
                    // Copy table number if dine-in
                    if (Order.Type == OrderType.DineIn && !string.IsNullOrEmpty(Order.TableNumber))
                    {
                        _orderService.UpdateCartTableNumber(Order.TableNumber);
                    }
                    
                    // Navigate to cart
                    await NavigationService.NavigateToAsync(Routes.Cart);
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync("Items added to cart");
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to reorder", ex);
                }
            }, "Adding items to cart...");
        }
        
        private string GetPaymentStatusText(PaymentStatus status)
        {
            return status switch
            {
                Shared.Models.Order.PaymentStatus.Pending => "Payment Pending",
                Shared.Models.Order.PaymentStatus.Paid => "Paid",
                Shared.Models.Order.PaymentStatus.Failed => "Payment Failed",
                Shared.Models.Order.PaymentStatus.Refunded => "Refunded",
                _ => status.ToString()
            };
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh order details
            if (!string.IsNullOrEmpty(_orderId))
            {
                LoadOrderAsync().ConfigureAwait(false);
            }
        }
    }
}