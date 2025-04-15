using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Order
{
    public partial class OrderHistoryViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        private readonly ISettingsService _settingsService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private ObservableCollection<OrderSummary> _orders;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _isLoading;
        
        [ObservableProperty]
        private bool _hasActiveOrder;
        
        [ObservableProperty]
        private OrderSummary _activeOrder;
        
        [ObservableProperty]
        private bool _isEmpty;
        
        [ObservableProperty]
        private string _filterStatus = "All";
        
        public ObservableCollection<string> FilterOptions { get; } = new ObservableCollection<string>
        {
            "All", "Active", "Completed", "Cancelled"
        };
        
        public OrderHistoryViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IOrderService orderService,
            ISettingsService settingsService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _orderService = orderService;
            _settingsService = settingsService;
            _signalRService = signalRService;
            
            Title = "My Orders";
            Orders = new ObservableCollection<OrderSummary>();
            
            // Subscribe to SignalR events
            _signalRService.OrderStatusChanged += OnOrderStatusChanged;
            _signalRService.NewOrder += OnNewOrder;
        }
        
        ~OrderHistoryViewModel()
        {
            // Unsubscribe from SignalR events
            _signalRService.OrderStatusChanged -= OnOrderStatusChanged;
            _signalRService.NewOrder -= OnNewOrder;
        }
        
        private void OnOrderStatusChanged(object sender, (string OrderId, OrderStatus Status) e)
        {
            // Refresh order list when any order status changes
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadOrdersAsync(true);
            });
        }
        
        private void OnNewOrder(object sender, string orderId)
        {
            // Refresh order list when a new order is placed
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadOrdersAsync(true);
            });
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await LoadOrdersAsync();
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadOrdersAsync(true);
        }
        
        private async Task LoadOrdersAsync(bool forceRefresh = false)
        {
            IsLoading = true;
            IsRefreshing = true;
            
            try
            {
                var userId = _settingsService.UserId;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }
                
                // Load order summaries
                var orderSummaries = await _orderService.GetOrdersByCustomerIdAsync(userId, forceRefresh);
                
                // Apply filter
                var filteredOrders = FilterOrders(orderSummaries);
                
                // Update collection
                Orders.Clear();
                foreach (var order in filteredOrders)
                {
                    Orders.Add(order);
                }
                
                // Check if there's an active order
                var active = orderSummaries.FirstOrDefault(o => 
                    o.Status != OrderStatus.Completed && 
                    o.Status != OrderStatus.Cancelled);
                
                HasActiveOrder = active != null;
                
                if (HasActiveOrder)
                {
                    ActiveOrder = active;
                }
                
                // Check if empty
                IsEmpty = Orders.Count == 0;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load orders", ex);
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }
        
        private List<OrderSummary> FilterOrders(List<OrderSummary> orders)
        {
            return FilterStatus switch
            {
                "Active" => orders.Where(o => 
                    o.Status != OrderStatus.Completed && 
                    o.Status != OrderStatus.Cancelled)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList(),
                
                "Completed" => orders.Where(o => o.Status == OrderStatus.Completed)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList(),
                
                "Cancelled" => orders.Where(o => o.Status == OrderStatus.Cancelled)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToList(),
                
                _ => orders.OrderByDescending(o => o.CreatedAt).ToList() // "All"
            };
        }
        
        partial void OnFilterStatusChanged(string value)
        {
            // Reload and filter orders
            LoadOrdersAsync().ConfigureAwait(false);
        }
        
        [RelayCommand]
        private async Task ViewOrderDetailAsync(OrderSummary order)
        {
            if (order == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "OrderId", order.Id }
            };
            
            await NavigationService.NavigateToAsync(Routes.OrderDetail, parameters);
        }
        
        [RelayCommand]
        private async Task PlaceNewOrderAsync()
        {
            await NavigationService.NavigateToAsync(Routes.Menu);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh order data
            LoadOrdersAsync().ConfigureAwait(false);
        }
    }
}