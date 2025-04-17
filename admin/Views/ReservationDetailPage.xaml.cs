using admin.ViewModels;

namespace admin.Views;

public partial class ReservationDetailPage : ContentPage
{
    private readonly ReservationDetailViewModel _viewModel;

    public ReservationDetailPage(ReservationDetailViewModel viewModel)
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