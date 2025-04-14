using client.ViewModels.Auth;
using Client.ViewModels.Auth;
using client.ViewModels.Menu;
using Client.ViewModels.Menu;
using client.ViewModels.Notification;
using client.ViewModels.Order;
using Client.ViewModels.Order;
using client.ViewModels.Reservation;
using client.ViewModels.Settings;

namespace Client.ViewModels.Base
{
    public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;
        
        public ViewModelLocator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        // Main ViewModel
        public MainViewModel Main => _serviceProvider.GetRequiredService<MainViewModel>();
        
        // Auth ViewModels
        public LoginViewModel Login => _serviceProvider.GetRequiredService<LoginViewModel>();
        public RegisterViewModel Register => _serviceProvider.GetRequiredService<RegisterViewModel>();
        public ProfileViewModel Profile => _serviceProvider.GetRequiredService<ProfileViewModel>();
        
        // Menu ViewModels
        public MenuViewModel Menu => _serviceProvider.GetRequiredService<MenuViewModel>();
        public MenuCategoryViewModel MenuCategory => _serviceProvider.GetRequiredService<MenuCategoryViewModel>();
        public MenuItemViewModel MenuItem => _serviceProvider.GetRequiredService<MenuItemViewModel>();
        public MenuItemDetailViewModel MenuItemDetail => _serviceProvider.GetRequiredService<MenuItemDetailViewModel>();
        
        // Order ViewModels
        public CartViewModel Cart => _serviceProvider.GetRequiredService<CartViewModel>();
        public OrderViewModel Order => _serviceProvider.GetRequiredService<OrderViewModel>();
        public OrderDetailViewModel OrderDetail => _serviceProvider.GetRequiredService<OrderDetailViewModel>();
        public OrderHistoryViewModel OrderHistory => _serviceProvider.GetRequiredService<OrderHistoryViewModel>();
        
        // Reservation ViewModels
        public ReservationViewModel Reservation => _serviceProvider.GetRequiredService<ReservationViewModel>();
        public ReservationDetailViewModel ReservationDetail => _serviceProvider.GetRequiredService<ReservationDetailViewModel>();
        public ReservationHistoryViewModel ReservationHistory => _serviceProvider.GetRequiredService<ReservationHistoryViewModel>();
        
        // Notification ViewModels
        public NotificationViewModel Notification => _serviceProvider.GetRequiredService<NotificationViewModel>();
        
        // Settings ViewModels
        public SettingsViewModel Settings => _serviceProvider.GetRequiredService<SettingsViewModel>();
    }
}