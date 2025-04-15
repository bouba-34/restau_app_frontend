using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Order
{
    public partial class OrderViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private Shared.Models.Order.Order _currentOrder;
        
        [ObservableProperty]
        private ObservableCollection<OrderItem> _orderItems;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _hasActiveOrder;
        
        [ObservableProperty]
        private int _estimatedWaitTime;
        
        [ObservableProperty]
        private bool _canCancel;
        
        public OrderViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IOrderService orderService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _orderService = orderService;
            _signalRService = signalRService;
            
            Title = "Current Order";
            OrderItems = new ObservableCollection<OrderItem>();
            
            // Subscribe to SignalR events
            _signalRService.OrderStatusChanged += OnOrderStatusChanged;
        }
        
        ~OrderViewModel()
        {
            // Unsubscribe from SignalR events
            _signalRService.OrderStatusChanged -= OnOrderStatusChanged;
        }
        
        private void OnOrderStatusChanged(object sender, (string OrderId, OrderStatus Status) e)
        {
            if (CurrentOrder != null && e.OrderId == CurrentOrder.Id)
            {
                // Update order status
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadCurrentOrderAsync();
                });
            }
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await LoadCurrentOrderAsync();
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadCurrentOrderAsync();
        }
        
        private async Task LoadCurrentOrderAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    // Check if user has an active order
                    bool hasActive = await _orderService.HasActiveOrderAsync();
                    HasActiveOrder = hasActive;
                    
                    if (hasActive)
                    {
                        // Load current order
                        CurrentOrder = await _orderService.GetCurrentOrderAsync();
                        
                        if (CurrentOrder != null)
                        {
                            // Update order items
                            OrderItems.Clear();
                            foreach (var item in CurrentOrder.Items)
                            {
                                OrderItems.Add(item);
                            }
                            
                            // Update estimated wait time
                            if (CurrentOrder.Status == OrderStatus.Placed || CurrentOrder.Status == OrderStatus.Preparing)
                            {
                                EstimatedWaitTime = await _orderService.GetEstimatedWaitTimeAsync(CurrentOrder.Id);
                            }
                            else
                            {
                                EstimatedWaitTime = 0;
                            }
                            
                            // Check if can cancel
                            CanCancel = CurrentOrder.Status == OrderStatus.Placed;
                        }
                    }
                    else
                    {
                        CurrentOrder = null;
                        OrderItems.Clear();
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load current order", ex);
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
            if (CurrentOrder == null)
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
                    bool success = await _orderService.CancelOrderAsync(CurrentOrder.Id);
                    
                    if (success)
                    {
                        // Reload current order
                        await LoadCurrentOrderAsync();
                        
                        // Show confirmation
                        await DialogService.DisplayToastAsync("Order cancelled successfully");
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
        private async Task ViewOrderHistoryAsync()
        {
            await NavigationService.NavigateToAsync(Routes.OrderHistory);
        }
        
        [RelayCommand]
        private async Task PlaceNewOrderAsync()
        {
            await NavigationService.NavigateToAsync(Routes.Menu);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data
            LoadCurrentOrderAsync().ConfigureAwait(false);
        }
    }
}