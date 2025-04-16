using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Drawing;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Painting;
using admin.ViewModels;
using SkiaSharp;

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
                HoverPaint = new SolidColorPaint(SKColors.White.WithAlpha(220)),
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                DataLabelsFormatter = point => $"{point.Context.Series.Name}: {point.PrimaryValue:C2}",
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
                DataLabelsFormatter = point => $"{point.PrimaryValue}",
                Name = "Orders"
            }
        };

        // Set up X and Y axes for the Orders by Hour chart
        _viewModel.OrdersXAxes = new List<ICartesianAxis>
        {
            new Axis
            {
                Labels = new List<string> { "N/A" },
                LabelsRotation = 45,
                ForceStepToMin = true,
                MinStep = 1
            }
        };

        _viewModel.OrdersYAxes = new List<ICartesianAxis>
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