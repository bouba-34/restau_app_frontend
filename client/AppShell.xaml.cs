using Client.ViewModels.Base;
using Client.Views.Auth;
using Client.Views.Menu;
using Client.Views.Order;

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
            Routing.RegisterRoute("login", typeof(LoginPage));
            Routing.RegisterRoute("register", typeof(RegisterPage));
            Routing.RegisterRoute("profile", typeof(ProfilePage));
            
            // Menu routes
            Routing.RegisterRoute("menu", typeof(MenuPage));
            Routing.RegisterRoute("menu/category", typeof(MenuCategoryPage));
            Routing.RegisterRoute("menu/item", typeof(MenuItemPage));
            Routing.RegisterRoute("menu/item/detail", typeof(MenuItemDetailPage));
            
            // Order routes
            Routing.RegisterRoute("cart", typeof(CartPage));
            Routing.RegisterRoute("order", typeof(OrderPage));
            Routing.RegisterRoute("order/detail", typeof(OrderDetailPage));
            Routing.RegisterRoute("order/history", typeof(OrderHistoryPage));
            
            // Reservation routes
            Routing.RegisterRoute("reservation", typeof(ReservationPage));
            Routing.RegisterRoute("reservation/detail", typeof(ReservationDetailPage));
            Routing.RegisterRoute("reservation/history", typeof(ReservationHistoryPage));
            
            // Notification routes
            Routing.RegisterRoute("notifications", typeof(NotificationPage));
            
            // Settings routes
            Routing.RegisterRoute("settings", typeof(SettingsPage));
        }
    }
}