namespace Shared.Constants
{
    public static class AppConstants
    {
        // Settings keys
        public const string SettingsApiBaseUrl = "ApiBaseUrl";
        public const string SettingsAuthToken = "AuthToken";
        public const string SettingsUserId = "UserId";
        public const string SettingsUsername = "Username";
        public const string SettingsUserType = "UserType";
        public const string SettingsUserEmail = "UserEmail";
        public const string SettingsRefreshToken = "RefreshToken";
        public const string SettingsTokenExpiration = "TokenExpiration";
        public const string SettingsDarkMode = "DarkMode";
        public const string SettingsNotificationsEnabled = "NotificationsEnabled";
        
        // Cache keys
        public const string CacheMenuCategories = "MenuCategories";
        public const string CacheMenuItems = "MenuItems";
        public const string CacheFeaturedItems = "FeaturedItems";
        public const string CacheUserOrders = "UserOrders_{0}"; // {0} = userId
        public const string CacheUserReservations = "UserReservations_{0}"; // {0} = userId
        
        // SignalR
        public const string SignalRHubUrl = "restauranthub";
        
        // Default values
        //public const string DefaultApiBaseUrl = "http://localhost:5238/";
        //public const string DefaultApiBaseUrl = "http://10.0.2.2:5238/";
        public const string DefaultApiBaseUrl = "http://192.168.11.104:5238/";
        
        // Cache durations (in minutes)
        public const int CacheDurationShort = 5;
        public const int CacheDurationMedium = 30;
        public const int CacheDurationLong = 240; // 4 hours
        
        // Order constants
        public const decimal DefaultTipPercentage = 0.15m; // 15%
        public const decimal TaxRate = 0.1m; // 10%
        
        // Image constants
        public const int ImageQuality = 80;
        public const int ThumbnailSize = 300;
        
        // Notification refresh interval (in seconds)
        public const int NotificationRefreshInterval = 30;
    }
}