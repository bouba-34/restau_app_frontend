using admin.ViewModels;

namespace admin.Views;

public partial class StaffDetailPage : ContentPage
{
    private readonly StaffDetailViewModel _viewModel;

    public StaffDetailPage(StaffDetailViewModel viewModel)
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