namespace admin.Controls;

public partial class StatisticsCard : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(StatisticsCard), string.Empty);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(string), typeof(StatisticsCard), string.Empty);

    public static readonly BindableProperty IconProperty =
        BindableProperty.Create(nameof(Icon), typeof(string), typeof(StatisticsCard), string.Empty);

    public static readonly BindableProperty IconBackgroundColorProperty =
        BindableProperty.Create(nameof(IconBackgroundColor), typeof(Color), typeof(StatisticsCard), Colors.Blue);

    public static readonly BindableProperty ShowComparisonProperty =
        BindableProperty.Create(nameof(ShowComparison), typeof(bool), typeof(StatisticsCard), false);

    public static readonly BindableProperty ComparisonTextProperty =
        BindableProperty.Create(nameof(ComparisonText), typeof(string), typeof(StatisticsCard), string.Empty);

    public static readonly BindableProperty ComparisonTextColorProperty =
        BindableProperty.Create(nameof(ComparisonTextColor), typeof(Color), typeof(StatisticsCard), Colors.Green);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Value
    {
        get => (string)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public Color IconBackgroundColor
    {
        get => (Color)GetValue(IconBackgroundColorProperty);
        set => SetValue(IconBackgroundColorProperty, value);
    }

    public bool ShowComparison
    {
        get => (bool)GetValue(ShowComparisonProperty);
        set => SetValue(ShowComparisonProperty, value);
    }

    public string ComparisonText
    {
        get => (string)GetValue(ComparisonTextProperty);
        set => SetValue(ComparisonTextProperty, value);
    }

    public Color ComparisonTextColor
    {
        get => (Color)GetValue(ComparisonTextColorProperty);
        set => SetValue(ComparisonTextColorProperty, value);
    }

    public StatisticsCard()
    {
        InitializeComponent();
    }
}