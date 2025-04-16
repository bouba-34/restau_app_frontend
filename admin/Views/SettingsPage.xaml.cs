using admin.ViewModels;

namespace admin.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
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

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleDarkModeCommand.Execute(null);
    }

    private void OnNotificationsToggled(object sender, ToggledEventArgs e)
    {
        _viewModel.ToggleNotificationsCommand.Execute(null);
    }
}