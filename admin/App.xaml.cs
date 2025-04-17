using System.Diagnostics;
using admin.Helpers;
using admin.Services.Interfaces;
using admin.Views;
using Shared.Services.Interfaces;

namespace admin;

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
            //MainPage = CreateAppShell();
            try
            {
                MainPage = ServiceHelper.GetService<AppShell>();
                //MainPage = new NavigationPage(ServiceHelper.GetService<MenuManagementPage>());
            }
            catch (Exception ex)
            {
                MainPage = new ContentPage
                {
                    Content = new Label { Text = $"Error: {ex.Message}" }
                };
            }
        }
        else
        {
            /*try
            {
                MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>());
            }
            catch (Exception ex)
            {
                MainPage = new ContentPage
                {
                    Content = new Label { Text = $"Error: {ex.Message}" }
                };
            }*/

            MainPage = new ContentPage { Content = new Label { Text = "Test Page" } };
        }
    }
    
    public static NavigationPage CreateAppShell()
    {
        /*var auth = ServiceHelper.GetService<IAuthService>();
        var settings = ServiceHelper.GetService<ISettingsService>();
        return new AppShell(auth, settings);*/
        return new NavigationPage(ServiceHelper.GetService<AppShell>());
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