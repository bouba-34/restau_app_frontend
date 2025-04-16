using admin.ViewModels;

namespace admin.Views;

public partial class ReservationsPage : ContentPage
{
    private readonly ReservationsViewModel _viewModel;

    public ReservationsPage(ReservationsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_viewModel.IsInitialized)
        {
            await _viewModel.InitializeAsync();
        }
    }
}