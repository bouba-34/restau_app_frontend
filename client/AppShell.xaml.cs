using Client.Constants;
using Client.ViewModels.Base;
using Client.Views.Auth;
using Client.Views.Menu;
using Client.Views.Notification;
using Client.Views.Order;
using Client.Views.Reservation;
using Client.Views.Settings;

namespace client
{
    public partial class AppShell : Shell
    {
        private readonly ViewModelLocator _viewModelLocator;
        
        public AppShell(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            _viewModelLocator = viewModelLocator;
            BindingContext = _viewModelLocator.Main;
            
            // Register routes for navigation
            RegisterRoutes();
        }
        
        private void RegisterRoutes()
        {
            // Auth routes
            Routing.RegisterRoute(Routes.Login, typeof(LoginPage));
            Routing.RegisterRoute(Routes.Register, typeof(RegisterPage));
            Routing.RegisterRoute(Routes.Profile, typeof(ProfilePage));
            
            // Menu routes
            Routing.RegisterRoute(Routes.Menu, typeof(MenuPage));
            Routing.RegisterRoute(Routes.MenuCategory, typeof(MenuCategoryPage));
            Routing.RegisterRoute(Routes.MenuItem, typeof(MenuItemPage));
            Routing.RegisterRoute(Routes.MenuItemDetail, typeof(MenuItemDetailPage));
            
            // Order routes
            Routing.RegisterRoute(Routes.Cart, typeof(CartPage));
            Routing.RegisterRoute(Routes.Order, typeof(OrderPage));
            Routing.RegisterRoute(Routes.OrderDetail, typeof(OrderDetailPage));
            Routing.RegisterRoute(Routes.OrderHistory, typeof(OrderHistoryPage));
            
            // Reservation routes
            Routing.RegisterRoute(Routes.Reservation, typeof(ReservationPage));
            Routing.RegisterRoute(Routes.ReservationDetail, typeof(ReservationDetailPage));
            Routing.RegisterRoute(Routes.ReservationHistory, typeof(ReservationHistoryPage));
            
            // Notification routes
            Routing.RegisterRoute(Routes.Notifications, typeof(NotificationPage));
            
            // Settings routes
            Routing.RegisterRoute(Routes.Settings, typeof(SettingsPage));
        }
    }
}