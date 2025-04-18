using System.Diagnostics;
using admin.Helpers;
using admin.Services.Interfaces;
using admin.Views;
using Shared.Services.Interfaces;

namespace admin;

/*public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var authService = ServiceHelper.GetService<IAuthService>();
        var settingsService = ServiceHelper.GetService<ISettingsService>();
        return new Window(new AppShell(authService, settingsService));
    }
}*/

public partial class App : Application
{
    private readonly ILocalSettingsService _localSettingsService;
    private readonly IThemeService _themeService;

    public App(ILocalSettingsService localSettingsService, IThemeService themeService)
    {
        InitializeComponent();

        _localSettingsService = localSettingsService;
        _themeService = themeService;

        // Set theme
        var isDarkMode = _localSettingsService.GetSetting<bool>("DarkMode", false);
        _themeService.SetTheme(isDarkMode);

        // Check if user is logged in
        var isLoggedIn = _localSettingsService.GetSetting<bool>("IsLoggedIn", false);

        if (isLoggedIn)
        {
            var authService = ServiceHelper.GetService<IAuthService>();
            var settingsService = ServiceHelper.GetService<ISettingsService>();
            if (authService == null || settingsService == null)
            {
                Debug.WriteLine("Required services are null");
                MainPage = new ContentPage { Content = new Label { Text = "Error: Required services not found" } };
                return;
            }
            MainPage = new AppShell(authService, settingsService);
            //var loginPage = ServiceHelper.GetService<LoginPage>();
            //MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>());
        }
        else
        {
            var loginPage = ServiceHelper.GetService<LoginPage>();
            MainPage = new NavigationPage(loginPage);
            //MainPage = new ContentPage { Content = new Label { Text = "just for testing" } };
        }
    }
    
    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
    }

    protected override void OnResume()
    {
        base.OnResume();
    }
}