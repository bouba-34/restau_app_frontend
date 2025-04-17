using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Shared.Services.Implementations;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Net.Http.Headers;
using admin.Helpers;
using admin.Services.Implementation;
using admin.Services.Interfaces;
using admin.ViewModels;
using admin.Views;
using SkiaSharp.Views.Maui.Controls.Hosting;
#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using WinRT.Interop;
#endif

namespace admin;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<TestApp>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome-Regular.ttf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesome-Solid.ttf", "FontAwesomeSolid");
                fonts.AddFont("FontAwesome-Brands.ttf", "FontAwesomeBrands");
            });

#if WINDOWS
        /*builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows => windows
                //.OnNativeMessage((app, args) => { })
                .OnWindowCreated(window =>
                {
                    window.ExtendsContentIntoTitleBar = false;
                    var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                    var nativeWindowHandle = new Windows.Win32.Foundation.HWND(handle);
                    var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
                    var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
                    appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 860));
                }));
        });*/
        builder.ConfigureLifecycleEvents(events =>
        {
            events.AddWindows(windows =>
                windows.OnWindowCreated(window =>
                {
                    var hwnd = WindowNative.GetWindowHandle(window);
                    var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
                    var appWindow = AppWindow.GetFromWindowId(windowId);
                    appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 860));
                }));
        });
#endif

        // Register services
        builder.Services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
        builder.Services.AddSingleton<IThemeService, ThemeService>();

        // Register HTTP client
        builder.Services.AddSingleton(serviceProvider =>
        {
            var httpClient = new HttpClient();
            var settingsService = serviceProvider.GetRequiredService<ISettingsService>();
            
            // Configure base address
            if (!string.IsNullOrEmpty(settingsService.ApiBaseUrl))
            {
                httpClient.BaseAddress = new Uri(settingsService.ApiBaseUrl);
            }

            // Configure default headers
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Configure token if available
            if (!string.IsNullOrEmpty(settingsService.AuthToken))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settingsService.AuthToken);
            }

            return httpClient;
        });

        // Register Shared services
        builder.Services.AddSingleton<ISettingsService, SettingsService>();
        builder.Services.AddSingleton<ICacheService, CacheService>();
        builder.Services.AddSingleton<ISignalRService, SignalRService>();
        builder.Services.AddSingleton<IReportService, ReportService>();

        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<IMenuService, MenuService>();
        builder.Services.AddSingleton<IOrderService, OrderService>();
        builder.Services.AddSingleton<IReservationService, ReservationService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();
        builder.Services.AddSingleton<IUserService, UserService>();
        
        // Register AppShell and its dependencies
        builder.Services.AddSingleton<AppShell>();

        // Register pages and view models
        RegisterPagesAndViewModels(builder.Services);

#if DEBUG
        builder.Logging.AddDebug();
#endif
        /*ServiceHelper.Initialize(builder.Build().Services);
        return builder.Build();*/
        var app = builder.Build();
        ServiceHelper.Initialize(app.Services);
        return app;
    }

    private static void RegisterPagesAndViewModels(IServiceCollection services)
    {
        Console.WriteLine("RegisterPagesAndViewModels");
        // Login
        services.AddTransient<LoginPage>();
        services.AddTransient<LoginViewModel>();

        // Dashboard
        services.AddTransient<DashboardPage>();
        services.AddTransient<DashboardViewModel>();

        // Orders
        services.AddTransient<OrdersPage>();
        services.AddTransient<OrdersViewModel>();
        services.AddTransient<OrderDetailPage>();
        services.AddTransient<OrderDetailViewModel>();

        // Menu
        services.AddTransient<MenuManagementPage>();
        services.AddTransient<MenuManagementViewModel>();
        services.AddTransient<MenuItemDetailPage>();
        services.AddTransient<MenuItemDetailViewModel>();

        // Reservations
        services.AddTransient<ReservationsPage>();
        services.AddTransient<ReservationsViewModel>();
        services.AddTransient<ReservationDetailPage>();
        services.AddTransient<ReservationDetailViewModel>();

        // Staff
        services.AddTransient<StaffManagementPage>();
        services.AddTransient<StaffManagementViewModel>();
        services.AddTransient<StaffDetailPage>();
        services.AddTransient<StaffDetailViewModel>();

        // Settings
        services.AddTransient<SettingsPage>();
        services.AddTransient<SettingsViewModel>();
    }
}