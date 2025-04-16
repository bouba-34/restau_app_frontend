using Shared.Models.Reservation;
using System.Windows.Input;

namespace admin.Controls;

public partial class ReservationCard : ContentView
{
    public static readonly BindableProperty ReservationProperty =
        BindableProperty.Create(nameof(Reservation), typeof(Reservation), typeof(ReservationCard), null, propertyChanged: OnReservationChanged);

    public static readonly BindableProperty ViewDetailsCommandProperty =
        BindableProperty.Create(nameof(ViewDetailsCommand), typeof(ICommand), typeof(ReservationCard), null);

    public static readonly BindableProperty UpdateStatusCommandProperty =
        BindableProperty.Create(nameof(UpdateStatusCommand), typeof(ICommand), typeof(ReservationCard), null);

    public static readonly BindableProperty NextActionTextProperty =
        BindableProperty.Create(nameof(NextActionText), typeof(string), typeof(ReservationCard), "Update Status");

    public static readonly BindableProperty ShowNextActionButtonProperty =
        BindableProperty.Create(nameof(ShowNextActionButton), typeof(bool), typeof(ReservationCard), true);

    public Reservation Reservation
    {
        get => (Reservation)GetValue(ReservationProperty);
        set => SetValue(ReservationProperty, value);
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

    public ReservationCard()
    {
        InitializeComponent();
    }

    private static void OnReservationChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (ReservationCard)bindable;
        if (control.Reservation != null)
        {
            control.UpdateNextActionText();
            control.UpdateShowNextActionButton();
        }
    }

    private void UpdateNextActionText()
    {
        NextActionText = Reservation.Status switch
        {
            ReservationStatus.Pending => "Confirm",
            ReservationStatus.Confirmed => "Mark as Completed",
            _ => "Update Status"
        };
    }

    private void UpdateShowNextActionButton()
    {
        ShowNextActionButton = Reservation.Status != ReservationStatus.Completed && 
                             Reservation.Status != ReservationStatus.Cancelled &&
                             Reservation.Status != ReservationStatus.NoShow;
    }
}