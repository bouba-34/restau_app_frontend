using admin.ViewModels;

namespace admin.Views;

public partial class MenuItemDetailPage : ContentPage
{
    private readonly MenuItemDetailViewModel _viewModel;

    public MenuItemDetailPage(MenuItemDetailViewModel viewModel)
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