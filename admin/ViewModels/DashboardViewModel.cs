using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using Shared.Models.Dashboard;
using Shared.Models.Order;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;
using admin.Views;

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

        [ObservableProperty]
        private ObservableCollection<ISeries> salesBySeries;

        [ObservableProperty]
        private ObservableCollection<ISeries> ordersByHourSeries;

        [ObservableProperty]
        private List<Axis> ordersXAxes;

        [ObservableProperty]
        private List<Axis> ordersYAxes;

        [ObservableProperty]
        private bool isSalesByCategoryEmpty = true;

        [ObservableProperty]
        private bool isOrdersByHourEmpty = true;

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
            UpdateSalesByCategoryChart();

            // Update orders by hour chart
            UpdateOrdersByHourChart();
        }

        private void UpdateSalesByCategoryChart()
        {
            if (DashboardSummary?.SalesByCategory == null || !DashboardSummary.SalesByCategory.Any())
            {
                IsSalesByCategoryEmpty = true;
                return;
            }

            IsSalesByCategoryEmpty = false;

            var colors = new SKColor[]
            {
                SKColors.DodgerBlue, SKColors.Orange, SKColors.Green, 
                SKColors.Magenta, SKColors.Gold, SKColors.Purple,
                SKColors.Brown, SKColors.Teal, SKColors.DeepPink
            };

            var values = new ObservableCollection<ISeries>();
            int colorIndex = 0;

            foreach (var category in DashboardSummary.SalesByCategory)
            {
                var pieValue = new PieSeries<ObservableValue>
                {
                    Name = category.Key,
                    Values = new ObservableCollection<ObservableValue> { new ObservableValue((double)category.Value) },
                    Fill = new SolidColorPaint(colors[colorIndex % colors.Length]),
                    Stroke = null,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                    DataLabelsFormatter = point => $"{point.Context.Series.Name}: ${point.Model:N2}"
                };

                values.Add(pieValue);
                colorIndex++;
            }

            SalesBySeries = values;
        }

        private void UpdateOrdersByHourChart()
        {
            if (DashboardSummary?.OrdersByHour == null || !DashboardSummary.OrdersByHour.Any())
            {
                IsOrdersByHourEmpty = true;
                return;
            }

            IsOrdersByHourEmpty = false;

            // Prepare values for orders by hour
            var hourValues = new ObservableCollection<ObservableValue>();
            var hourLabels = new List<string>();

            // Sort by hour
            var orderedHours = DashboardSummary.OrdersByHour.OrderBy(h => h.Key).ToList();

            foreach (var hourData in orderedHours)
            {
                hourValues.Add(new ObservableValue(hourData.Value));
                hourLabels.Add($"{hourData.Key:00}:00");
            }

            // Create the column series
            var columnSeries = new ColumnSeries<ObservableValue>
            {
                Name = "Orders",
                Values = hourValues,
                Fill = new SolidColorPaint(SKColors.DodgerBlue),
                Stroke = null,
                DataLabelsFormatter = point => $"{point.Model}"
            };

            // Set the series
            OrdersByHourSeries = new ObservableCollection<ISeries> { columnSeries };

            // Configure X axis
            OrdersXAxes = new List<Axis>
            {
                new Axis
                {
                    Labels = hourLabels,
                    LabelsRotation = 45,
                    ForceStepToMin = true,
                    MinStep = 1
                }
            };

            // Configure Y axis
            OrdersYAxes = new List<Axis>
            {
                new Axis
                {
                    ForceStepToMin = true,
                    MinStep = 1,
                    MinLimit = 0
                }
            };
        }

        [RelayCommand]
        private async Task ViewAllOrdersAsync()
        {
            //await Shell.Current.GoToAsync("//Orders");
            await Shell.Current.GoToAsync(nameof(OrdersPage));
        }

        [RelayCommand]
        private async Task ViewAllReservationsAsync()
        {
            //await Shell.Current.GoToAsync("//Reservations");
            await Shell.Current.GoToAsync(nameof(ReservationsPage));
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
        private async Task ViewReservationDetailsAsync(string reservationId)
        {
            if (string.IsNullOrEmpty(reservationId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "ReservationId", reservationId }
            };

            //await Shell.Current.GoToAsync($"ReservationDetailPage", parameters);
            await Shell.Current.GoToAsync(nameof(ReservationDetailPage), parameters);
        }
    }
}