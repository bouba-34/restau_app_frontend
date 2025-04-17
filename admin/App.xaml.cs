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
        
        try
        {
            Debug.WriteLine("Setting main page...");
            
            if (isLoggedIn)
            {
                try
                {
                    Debug.WriteLine("Trying to create AppShell...");
                    // Try to create required services first to check they exist
                    var authService = ServiceHelper.GetService<IAuthService>();
                    var settingsService = ServiceHelper.GetService<ISettingsService>();
                    
                    if (authService == null || settingsService == null)
                    {
                        Debug.WriteLine("Required services are null");
                        MainPage = new ContentPage { Content = new Label { Text = "Error: Required services not found" } };
                        return;
                    }
                    
                    Debug.WriteLine("Creating AppShell...");
                    var appShell = new AppShell(authService, settingsService);
                    MainPage = appShell;
                    Debug.WriteLine("AppShell created successfully");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error creating AppShell: {ex.Message}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    MainPage = new ContentPage
                    {
                        Content = new VerticalStackLayout
                        {
                            Children = 
                            {
                                new Label { Text = $"Error: {ex.Message}", TextColor = Colors.Red },
                                new Label { Text = $"Stack trace: {ex.StackTrace}", FontSize = 12 }
                            },
                            VerticalOptions = LayoutOptions.Center,
                            Padding = 20
                        }
                    };
                }
            }
            else
            {
                // For now, just show the test page for debugging
                Debug.WriteLine("Showing test page");
                MainPage = new ContentPage 
                { 
                    Content = new VerticalStackLayout
                    {
                        Children =
                        {
                            new Label { Text = "Test Page - Not Logged In", HorizontalOptions = LayoutOptions.Center },
                            new Button 
                            { 
                                Text = "Go to Login Page",
                                Command = new Command(async () => 
                                {
                                    try 
                                    {
                                        var loginPage = ServiceHelper.GetService<LoginPage>();
                                        MainPage = new NavigationPage(loginPage);
                                    }
                                    catch (Exception ex)
                                    {
                                        await Current.MainPage.DisplayAlert("Error", $"Could not navigate to login: {ex.Message}", "OK");
                                    }
                                })
                            }
                        },
                        VerticalOptions = LayoutOptions.Center
                    } 
                };
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unhandled error in App constructor: {ex.Message}");
            Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            MainPage = new ContentPage
            {
                Content = new Label { Text = $"Critical Error: {ex.Message}" }
            };
        }
    }
    
    public static NavigationPage CreateAppShell()
    {
        try
        {
            Debug.WriteLine("CreateAppShell called");
            var auth = ServiceHelper.GetService<IAuthService>();
            var settings = ServiceHelper.GetService<ISettingsService>();
            
            if (auth == null || settings == null)
            {
                Debug.WriteLine("Services not available in CreateAppShell");
                return new NavigationPage(new ContentPage { Content = new Label { Text = "Error: Services not found" } });
            }
            
            return new NavigationPage(new AppShell(auth, settings));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in CreateAppShell: {ex.Message}");
            return new NavigationPage(new ContentPage { Content = new Label { Text = $"Error: {ex.Message}" } });
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