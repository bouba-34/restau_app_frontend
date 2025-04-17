using admin.ViewModels;

namespace admin.Views;

public partial class StaffManagementPage : ContentPage
{
    private readonly StaffManagementViewModel _viewModel;

    public StaffManagementPage(StaffManagementViewModel viewModel)
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