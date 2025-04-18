using System.Net.Http.Headers;
using admin.Helpers;
using admin.Services.Implementation;
using admin.Services.Interfaces;
using admin.ViewModels;
using admin.Views;
using CommunityToolkit.Maui;
using LiveChartsCore.SkiaSharpView.Maui;
using Microsoft.Extensions.Logging;
using Shared.Services.Implementations;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using SkiaSharp.Views.Maui.Controls.Hosting;


namespace admin;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseLiveCharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("FontAwesome-Regular.ttf", "FontAwesomeRegular");
                fonts.AddFont("FontAwesome-Solid.ttf", "FontAwesomeSolid");
                fonts.AddFont("FontAwesome-Brands.ttf", "FontAwesomeBrands");
            });
        
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
        //builder.Services.AddSingleton<AppShell>();

        // Register pages and view models
        RegisterPagesAndViewModels(builder.Services);




#if DEBUG
        builder.Logging.AddDebug();
#endif
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