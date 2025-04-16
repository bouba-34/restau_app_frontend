using admin.ViewModels;

namespace admin.Views;

public partial class MenuManagementPage : ContentPage
{
    private readonly MenuManagementViewModel _viewModel;

    public MenuManagementPage(MenuManagementViewModel viewModel)
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