using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


namespace admin;

public partial class MainPage : ContentPage
{

    public ISeries[] Series { get; set; }
    public Axis[] XAxes { get; set; }
    public Axis[] YAxes { get; set; }

    public MainPage()
    {
        InitializeComponent();

        Series = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = new double[] { 2, 5, 4, 8, 6 },
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.DodgerBlue, 4)
            }
        };

        XAxes = [new Axis { Labels = new[] { "Lun", "Mar", "Mer", "Jeu", "Ven" } }];
        YAxes = [new Axis { MinLimit = 0, MaxLimit = 10 }];

        BindingContext = this;
    }
    
}