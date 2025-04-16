using admin.Helpers;
using admin.Views;

namespace admin;
using Shared.Services.Interfaces;
using System.Windows.Input;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;
    private readonly ISettingsService _settingsService;

    public ICommand LogoutCommand { get; }
    public string Username => _settingsService.Username;

    public AppShell(IAuthService authService, ISettingsService settingsService)
    {
        InitializeComponent();

        _authService = authService;
        _settingsService = settingsService;

        // Register routes for navigation
        Routing.RegisterRoute(nameof(OrderDetailPage), typeof(OrderDetailPage));
        Routing.RegisterRoute(nameof(MenuItemDetailPage), typeof(MenuItemDetailPage));
        Routing.RegisterRoute(nameof(ReservationDetailPage), typeof(ReservationDetailPage));
        Routing.RegisterRoute(nameof(StaffDetailPage), typeof(StaffDetailPage));

        LogoutCommand = new Command(OnLogoutClicked);
        
        // Set binding context for the flyout items
        BindingContext = this;
    }

    private async void OnLogoutClicked()
    {
        bool confirmed = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        
        if (confirmed)
        {
            await _authService.LogoutAsync();
            //Application.Current.MainPage = new NavigationPage(new LoginPage());
            Application.Current.MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>());
        }
    }
}