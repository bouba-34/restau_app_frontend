using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using admin.ViewModels;

namespace admin.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Initialize chart series
        InitializeCharts();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_viewModel.IsInitialized)
        {
            await _viewModel.InitializeAsync();
        }
    }

    private void InitializeCharts()
    {
        // Initialize PieChart series for Sales by Category
        _viewModel.SalesBySeries = new ObservableCollection<ISeries>
        {
            new PieSeries<ObservableValue>
            {
                Values = new ObservableCollection<ObservableValue> 
                {
                    new ObservableValue(0)
                },
                Fill = new SolidColorPaint(SKColors.Blue),
                Stroke = null,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Context.Series.Name}: {point.Model:C2}",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                Name = "No Data"
            }
        };

        // Initialize series for Orders by Hour
        _viewModel.OrdersByHourSeries = new ObservableCollection<ISeries>
        {
            new ColumnSeries<ObservableValue>
            {
                Values = new ObservableCollection<ObservableValue> 
                {
                    new ObservableValue(0)
                },
                Fill = new SolidColorPaint(SKColors.Blue),
                Stroke = null,
                DataLabelsFormatter = point => $"{point.Model}",
                Name = "Orders"
            }
        };

        // Set up X and Y axes for the Orders by Hour chart
        _viewModel.OrdersXAxes = new List<Axis>
        {
            new Axis
            {
                Labels = new List<string> { "N/A" },
                LabelsRotation = 45,
                ForceStepToMin = true,
                MinStep = 1
            }
        };

        _viewModel.OrdersYAxes = new List<Axis>
        {
            new Axis
            {
                ForceStepToMin = true,
                MinStep = 1,
                MinLimit = 0
            }
        };

        // Set initial empty state flags
        _viewModel.IsSalesByCategoryEmpty = true;
        _viewModel.IsOrdersByHourEmpty = true;
    }
}