using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;
using admin.Views;

namespace admin.ViewModels
{
    public partial class OrdersViewModel : BaseViewModel
    {
        private readonly IOrderService _orderService;

        [ObservableProperty]
        private ObservableCollection<Order> orders = new();

        [ObservableProperty]
        private OrderStatus selectedStatus = OrderStatus.Placed;

        [ObservableProperty]
        private bool isFilterByDate;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime endDate = DateTime.Today;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private ObservableCollection<OrderStatus> orderStatusOptions = new(Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>());

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int totalOrders;

        public OrdersViewModel(IOrderService orderService)
        {
            Title = "Orders";
            _orderService = orderService;

            // Set default filter to today's date
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await RefreshAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await LoadOrdersAsync();
            IsRefreshing = false;
        }

        private async Task LoadOrdersAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                List<Order> ordersList;

                if (IsFilterByDate)
                {
                    // Get orders by date range
                    ordersList = await _orderService.GetOrdersByDateRangeAsync(StartDate, EndDate);
                }
                else
                {
                    // Get orders by status
                    ordersList = await _orderService.GetOrdersByStatusAsync(SelectedStatus);
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    ordersList = ordersList.Where(o => 
                        o.Id.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        o.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                Orders.Clear();
                foreach (var order in ordersList)
                {
                    Orders.Add(order);
                }

                TotalOrders = Orders.Count;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load orders: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            await LoadOrdersAsync();
        }

        [RelayCommand]
        private async Task ViewOrderDetailsAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "OrderId", orderId }
            };

            //await Shell.Current.GoToAsync($"OrderDetailPage", parameters);
            await Shell.Current.GoToAsync(nameof(OrderDetailPage), parameters);
        }

        [RelayCommand]
        private async Task UpdateOrderStatusAsync(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
                return;

            var order = Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
                return;

            // Get next status based on current status
            var nextStatus = GetNextOrderStatus(order.Status);

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _orderService.UpdateOrderStatusAsync(orderId, nextStatus);
                if (success)
                {
                    // Update the order in the collection
                    order.Status = nextStatus;
                    
                    // Refresh the collection to update UI
                    int index = Orders.IndexOf(order);
                    if (index >= 0)
                    {
                        Orders.RemoveAt(index);
                        Orders.Insert(index, order);
                    }
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

        [RelayCommand]
        private void ToggleFilterMode()
        {
            IsFilterByDate = !IsFilterByDate;
        }

        partial void OnSelectedStatusChanged(OrderStatus value)
        {
            if (IsInitialized && !IsFilterByDate)
            {
                LoadOrdersAsync().ConfigureAwait(false);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (IsInitialized && !string.IsNullOrEmpty(value) && value.Length > 2)
            {
                LoadOrdersAsync().ConfigureAwait(false);
            }
            else if (IsInitialized && string.IsNullOrEmpty(value))
            {
                LoadOrdersAsync().ConfigureAwait(false);
            }
        }
    }
}