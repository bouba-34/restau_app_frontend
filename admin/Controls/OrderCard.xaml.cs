using Shared.Models.Order;
using System.Windows.Input;

namespace admin.Controls;

public partial class OrderCard : ContentView
{
    public static readonly BindableProperty OrderProperty =
        BindableProperty.Create(nameof(Order), typeof(Order), typeof(OrderCard), null, propertyChanged: OnOrderChanged);

    public static readonly BindableProperty ViewDetailsCommandProperty =
        BindableProperty.Create(nameof(ViewDetailsCommand), typeof(ICommand), typeof(OrderCard), null);

    public static readonly BindableProperty UpdateStatusCommandProperty =
        BindableProperty.Create(nameof(UpdateStatusCommand), typeof(ICommand), typeof(OrderCard), null);

    public static readonly BindableProperty NextActionTextProperty =
        BindableProperty.Create(nameof(NextActionText), typeof(string), typeof(OrderCard), "Update Status");

    public static readonly BindableProperty ShowNextActionButtonProperty =
        BindableProperty.Create(nameof(ShowNextActionButton), typeof(bool), typeof(OrderCard), true);

    public Order Order
    {
        get => (Order)GetValue(OrderProperty);
        set => SetValue(OrderProperty, value);
    }

    public ICommand ViewDetailsCommand
    {
        get => (ICommand)GetValue(ViewDetailsCommandProperty);
        set => SetValue(ViewDetailsCommandProperty, value);
    }

    public ICommand UpdateStatusCommand
    {
        get => (ICommand)GetValue(UpdateStatusCommandProperty);
        set => SetValue(UpdateStatusCommandProperty, value);
    }

    public string NextActionText
    {
        get => (string)GetValue(NextActionTextProperty);
        set => SetValue(NextActionTextProperty, value);
    }

    public bool ShowNextActionButton
    {
        get => (bool)GetValue(ShowNextActionButtonProperty);
        set => SetValue(ShowNextActionButtonProperty, value);
    }

    public OrderCard()
    {
        InitializeComponent();
    }

    private static void OnOrderChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (OrderCard)bindable;
        if (control.Order != null)
        {
            control.UpdateNextActionText();
            control.UpdateShowNextActionButton();
        }
    }

    private void UpdateNextActionText()
    {
        NextActionText = Order.Status switch
        {
            OrderStatus.Placed => "Start Preparing",
            OrderStatus.Preparing => "Mark as Ready",
            OrderStatus.Ready => "Mark as Served",
            OrderStatus.Served => "Complete Order",
            _ => "Update Status"
        };
    }

    private void UpdateShowNextActionButton()
    {
        ShowNextActionButton = Order.Status != OrderStatus.Completed && Order.Status != OrderStatus.Cancelled;
    }
}