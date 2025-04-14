using Microsoft.Extensions.Logging;
using Client.Services;
using Client.ViewModels;
using Client.ViewModels.Auth;
using Client.ViewModels.Base;
using Client.ViewModels.Menu;
using Client.ViewModels.Notification;
using Client.ViewModels.Order;
using Client.ViewModels.Reservation;
using Client.ViewModels.Settings;
using Client.Views.Auth;
using Client.Views.Menu;
using Client.Views.Notification;
using Client.Views.Order;
using Client.Views.Reservation;
using Client.Views.Settings;
using Shared.Services.Implementations;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Net.Http.Headers;
using client.Services;
using client.ViewModels.Auth;
using client.ViewModels.Menu;
using client.ViewModels.Notification;
using client.ViewModels.Order;
using client.ViewModels.Reservation;
using client.ViewModels.Settings;

namespace Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("FontAwesome-Regular.ttf", "FontAwesomeRegular");
                    fonts.AddFont("FontAwesome-Solid.ttf", "FontAwesomeSolid");
                    fonts.AddFont("FontAwesome-Brands.ttf", "FontAwesomeBrands");
                });

            // Register Services
            RegisterServices(builder.Services);
            
            // Register ViewModels
            RegisterViewModels(builder.Services);
            
            // Register Views
            RegisterViews(builder.Services);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        
        private static void RegisterServices(IServiceCollection services)
        {
            // Platform Services
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<ICacheService, CacheService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IConnectivityService, ConnectivityService>();
            
            // HTTP Client
            services.AddSingleton<HttpClient>(sp => 
            {
                var settingsService = sp.GetRequiredService<ISettingsService>();
                var httpClient = new HttpClient();
                
                if (!string.IsNullOrEmpty(settingsService.AuthToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", settingsService.AuthToken);
                }
                
                return httpClient;
            });
            
            // API Services
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IMenuService, MenuService>();
            services.AddSingleton<IOrderService, OrderService>();
            services.AddSingleton<IReservationService, ReservationService>();
            services.AddSingleton<INotificationService, NotificationService>();
            
            // SignalR
            services.AddSingleton<ISignalRService, SignalRService>();
        }
        
        private static void RegisterViewModels(IServiceCollection services)
        {
            // Register the ViewModelLocator
            services.AddSingleton<ViewModelLocator>();
            
            // Base ViewModels
            services.AddTransient<MainViewModel>();
            
            // Auth ViewModels
            services.AddTransient<LoginViewModel>();
            services.AddTransient<RegisterViewModel>();
            services.AddTransient<ProfileViewModel>();
            
            // Menu ViewModels
            services.AddTransient<MenuViewModel>();
            services.AddTransient<MenuCategoryViewModel>();
            services.AddTransient<MenuItemViewModel>();
            services.AddTransient<MenuItemDetailViewModel>();
            
            // Order ViewModels
            services.AddTransient<CartViewModel>();
            services.AddTransient<OrderViewModel>();
            services.AddTransient<OrderDetailViewModel>();
            services.AddTransient<OrderHistoryViewModel>();
            
            // Reservation ViewModels
            services.AddTransient<ReservationViewModel>();
            services.AddTransient<ReservationDetailViewModel>();
            services.AddTransient<ReservationHistoryViewModel>();
            
            // Notification ViewModels
            services.AddTransient<NotificationViewModel>();
            
            // Settings ViewModels
            services.AddTransient<SettingsViewModel>();
        }
        
        private static void RegisterViews(IServiceCollection services)
        {
            // Auth Views
            services.AddTransient<LoginPage>();
            services.AddTransient<RegisterPage>();
            services.AddTransient<ProfilePage>();
            
            // Menu Views
            services.AddTransient<MenuPage>();
            services.AddTransient<MenuCategoryPage>();
            services.AddTransient<MenuItemPage>();
            services.AddTransient<MenuItemDetailPage>();
            
            // Order Views
            services.AddTransient<CartPage>();
            services.AddTransient<OrderPage>();
            services.AddTransient<OrderDetailPage>();
            services.AddTransient<OrderHistoryPage>();
            
            // Reservation Views
            services.AddTransient<ReservationPage>();
            services.AddTransient<ReservationDetailPage>();
            services.AddTransient<ReservationHistoryPage>();
            
            // Notification Views
            services.AddTransient<NotificationPage>();
            
            // Settings Views
            services.AddTransient<SettingsPage>();
        }
    }
}