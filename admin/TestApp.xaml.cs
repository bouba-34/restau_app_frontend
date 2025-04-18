using System.Diagnostics;
using admin.Helpers;
using admin.Services.Interfaces;
using admin.ViewModels;
using admin.Views;
using Shared.Services.Interfaces;

namespace admin;

public class TestApp : Application
{
    public TestApp()
    {
        // Set up a simple page with debug information
        MainPage = new ContentPage
        {
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Spacing = 10,
                    Padding = 20,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Label { Text = "Diagnostic Test Page", FontSize = 24, HorizontalOptions = LayoutOptions.Center },
                        new Button 
                        { 
                            Text = "Test Services",
                            Command = new Command(TestServices)
                        },
                        new Button
                        {
                            Text = "Test Dashboard Page",
                            Command = new Command(TestDashboardPage)
                        },
                        new Button
                        {
                            Text = "Test Menu Page",
                            Command = new Command(TestMenuPage)
                        },
                        new Button
                        {
                            Text = "Test Orders Page",
                            Command = new Command(TestOrdersPage)
                        },
                        new Button
                        {
                            Text = "Test Reservations Page",
                            Command = new Command(TestReservationsPage)
                        },
                        new Button
                        {
                            Text = "Test Settings Page",
                            Command = new Command(TestSettingsPage)
                        },
                        new Button
                        {
                            Text = "Test App Shell",
                            Command = new Command(TestFullAppShell)
                        }
                    }
                }
            }
        };
    }

    private async void TestServices()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Service Registration Test Results:");
        
        try
        {
            // Test each service one by one
            sb.AppendLine("\nTesting LocalSettingsService:");
            var localSettings = ServiceHelper.GetService<ILocalSettingsService>();
            sb.AppendLine(localSettings != null ? "✓ Success" : "✗ Failed");
            
            sb.AppendLine("\nTesting ThemeService:");
            var themeService = ServiceHelper.GetService<IThemeService>();
            sb.AppendLine(themeService != null ? "✓ Success" : "✗ Failed");
            
            sb.AppendLine("\nTesting SettingsService:");
            var settingsService = ServiceHelper.GetService<ISettingsService>();
            sb.AppendLine(settingsService != null ? "✓ Success" : "✗ Failed");
            
            sb.AppendLine("\nTesting AuthService:");
            var authService = ServiceHelper.GetService<IAuthService>();
            sb.AppendLine(authService != null ? "✓ Success" : "✗ Failed");
            
            sb.AppendLine("\nTesting ReportService:");
            var reportService = ServiceHelper.GetService<IReportService>();
            sb.AppendLine(reportService != null ? "✓ Success" : "✗ Failed");
            
            sb.AppendLine("\nTesting AppShell:");
            try 
            {
                var appShell = new AppShell(authService, settingsService);
                sb.AppendLine("✓ AppShell creation successful");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"✗ AppShell creation failed: {ex.Message}");
                sb.AppendLine($"Stack trace: {ex.StackTrace}");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"\nError during testing: {ex.Message}");
            sb.AppendLine($"Stack trace: {ex.StackTrace}");
        }
        
        await Application.Current.MainPage.DisplayAlert("Test Results", sb.ToString(), "OK");
    }

    private async void TestDashboardPage()
    {
        await TestPage<DashboardViewModel, DashboardPage>("Dashboard Page");
    }
    
    private async void TestMenuPage()
    {
        await TestPage<MenuManagementViewModel, MenuManagementPage>("Menu Management Page");
    }
    
    private async void TestOrdersPage()
    {
        await TestPage<OrdersViewModel, OrdersPage>("Orders Page");
    }
    
    private async void TestReservationsPage()
    {
        await TestPage<ReservationsViewModel, ReservationsPage>("Reservations Page");
    }
    
    private async void TestSettingsPage()
    {
        await TestPage<SettingsViewModel, SettingsPage>("Settings Page");
    }
    
    private async Task TestPage<TViewModel, TPage>(string pageName) 
        where TViewModel : BaseViewModel 
        where TPage : ContentPage
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"{pageName} Test Results:");
        
        try
        {
            // Test ViewModel creation
            sb.AppendLine($"\nTesting {typeof(TViewModel).Name}:");
            var viewModel = ServiceHelper.GetService<TViewModel>();
            
            if (viewModel != null)
            {
                sb.AppendLine("✓ ViewModel created successfully");
                
                // Test Page creation
                sb.AppendLine($"\nTesting {typeof(TPage).Name}:");
                var page = ServiceHelper.GetService<TPage>();
                
                if (page != null)
                {
                    sb.AppendLine("✓ Page created successfully");
                    
                    // Test initialization
                    sb.AppendLine("\nTesting Page Initialization:");
                    try
                    {
                        await viewModel.InitializeAsync();
                        sb.AppendLine("✓ ViewModel initialized successfully");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"✗ ViewModel initialization failed: {ex.Message}");
                        sb.AppendLine($"Stack trace: {ex.StackTrace}");
                    }
                }
                else
                {
                    sb.AppendLine("✗ Page creation failed");
                }
            }
            else
            {
                sb.AppendLine("✗ ViewModel creation failed");
            }
        }
        catch (Exception ex)
        {
            sb.AppendLine($"\nError during testing: {ex.Message}");
            sb.AppendLine($"Stack trace: {ex.StackTrace}");
        }
        
        await Application.Current.MainPage.DisplayAlert("Test Results", sb.ToString(), "OK");
    }
    
    private async void TestFullAppShell()
    {
        try
        {
            var authService = ServiceHelper.GetService<IAuthService>();
            var settingsService = ServiceHelper.GetService<ISettingsService>();
            
            if (authService != null && settingsService != null)
            {
                var appShell = new AppShell(authService, settingsService);
                Application.Current.MainPage = appShell;
                await Application.Current.MainPage.DisplayAlert("Success", "AppShell loaded successfully", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Required services not found", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load AppShell: {ex.Message}", "OK");
        }
    }
}