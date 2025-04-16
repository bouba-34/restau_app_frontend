using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Dashboard;
using Shared.Models.Order;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel
    {
        private readonly IReportService _reportService;
        private readonly IOrderService _orderService;
        private readonly IReservationService _reservationService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private DashboardSummary dashboardSummary;

        [ObservableProperty]
        private ObservableCollection<Order> activeOrders = new();

        [ObservableProperty]
        private ObservableCollection<Reservation> upcomingReservations = new();

        [ObservableProperty]
        private int totalActiveOrders;

        [ObservableProperty]
        private int totalUpcomingReservations;

        [ObservableProperty]
        private bool isLoadingSummary;

        [ObservableProperty]
        private bool isLoadingOrders;

        [ObservableProperty]
        private bool isLoadingReservations;

        public ObservableCollection<ChartData> SalesByCategory { get; } = new();
        public ObservableCollection<ChartData> OrdersByHour { get; } = new();

        public DashboardViewModel(
            IReportService reportService,
            IOrderService orderService,
            IReservationService reservationService,
            ISettingsService settingsService)
        {
            Title = "Dashboard";
            Subtitle = "Welcome, " + settingsService.Username;

            _reportService = reportService;
            _orderService = orderService;
            _reservationService = reservationService;
            _settingsService = settingsService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await RefreshAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await Task.WhenAll(
                LoadDashboardSummaryAsync(),
                LoadActiveOrdersAsync(),
                LoadUpcomingReservationsAsync()
            );
            IsRefreshing = false;
        }

        private async Task LoadDashboardSummaryAsync()
        {
            try
            {
                IsLoadingSummary = true;
                ClearError();

                DashboardSummary = await _reportService.GetDashboardSummaryAsync();
                UpdateCharts();
            }
            catch (Exception ex)
            {
                ShowError("Failed to load dashboard summary: " + ex.Message);
            }
            finally
            {
                IsLoadingSummary = false;
            }
        }

        private async Task LoadActiveOrdersAsync()
        {
            try
            {
                IsLoadingOrders = true;
                ClearError();

                var orders = await _orderService.GetActiveOrdersAsync();
                ActiveOrders.Clear();
                foreach (var order in orders.Take(5))
                {
                    ActiveOrders.Add(order);
                }
                TotalActiveOrders = orders.Count;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load active orders: " + ex.Message);
            }
            finally
            {
                IsLoadingOrders = false;
            }
        }

        private async Task LoadUpcomingReservationsAsync()
        {
            try
            {
                IsLoadingReservations = true;
                ClearError();

                var reservations = await _reservationService.GetUpcomingReservationsAsync();
                UpcomingReservations.Clear();
                foreach (var reservation in reservations.Take(5))
                {
                    UpcomingReservations.Add(reservation);
                }
                TotalUpcomingReservations = reservations.Count;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load upcoming reservations: " + ex.Message);
            }
            finally
            {
                IsLoadingReservations = false;
            }
        }

        private void UpdateCharts()
        {
            if (DashboardSummary == null)
                return;

            // Update sales by category chart
            SalesByCategory.Clear();
            if (DashboardSummary.SalesByCategory != null)
            {
                foreach (var category in DashboardSummary.SalesByCategory)
                {
                    SalesByCategory.Add(new ChartData(category.Key, (double)category.Value));
                }
            }

            // Update orders by hour chart
            OrdersByHour.Clear();
            if (DashboardSummary.OrdersByHour != null)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    if (DashboardSummary.OrdersByHour.TryGetValue(hour, out int count))
                    {
                        OrdersByHour.Add(new ChartData($"{hour}:00", count));
                    }
                    else
                    {
                        OrdersByHour.Add(new ChartData($"{hour}:00", 0));
                    }
                }
            }
        }

        [RelayCommand]
        private async Task ViewAllOrdersAsync()
        {
            await Shell.Current.GoToAsync("//Orders");
        }

        [RelayCommand]
        private async Task ViewAllReservationsAsync()
        {
            await Shell.Current.GoToAsync("//Reservations");
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

            await Shell.Current.GoToAsync($"OrderDetailPage", parameters);
        }

        [RelayCommand]
        private async Task ViewReservationDetailsAsync(string reservationId)
        {
            if (string.IsNullOrEmpty(reservationId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "ReservationId", reservationId }
            };

            await Shell.Current.GoToAsync($"ReservationDetailPage", parameters);
        }
    }

    public class ChartData
    {
        public string Category { get; set; }
        public double Value { get; set; }

        public ChartData(string category, double value)
        {
            Category = category;
            Value = value;
        }
    }
}