namespace Shared.Constants
{
    public static class ApiEndpoints
    {
        // Auth endpoints
        public const string Login = "api/Auth/login";
        public const string Register = "api/Auth/register";
        public const string ChangePassword = "api/Auth/change-password";
        public const string GetProfile = "api/Auth/profile";
        
        // Menu endpoints
        public const string GetCategories = "api/Menu/categories";
        public const string GetCategory = "api/Menu/categories/{0}";
        public const string CreateCategory = "api/Menu/categories";
        public const string UpdateCategory = "api/Menu/categories/{0}";
        public const string DeleteCategory = "api/Menu/categories/{0}";
        
        public const string GetMenuItems = "api/Menu/items";
        public const string GetMenuItemsByCategory = "api/Menu/items/category/{0}";
        public const string GetFeaturedMenuItems = "api/Menu/items/featured";
        public const string GetMenuItem = "api/Menu/items/{0}";
        public const string SearchMenuItems = "api/Menu/items/search?query={0}";
        public const string CreateMenuItem = "api/Menu/items";
        public const string UpdateMenuItem = "api/Menu/items/{0}";
        public const string UpdateMenuItemAvailability = "api/Menu/items/{0}/availability";
        public const string DeleteMenuItem = "api/Menu/items/{0}";
        public const string DeleteCategoryImage = "api/Menu/categories/{0}/image";
        public const string DeleteMenuItemImage = "api/Menu/items/{0}/image";
        
        // Order endpoints
        public const string GetOrder = "api/Order/{0}";
        public const string GetOrdersByCustomer = "api/Order/customer/{0}";
        public const string GetActiveOrders = "api/Order/active";
        public const string GetOrdersByStatus = "api/Order/status/{0}";
        public const string GetOrdersByDateRange = "api/Order/date-range?startDate={0}&endDate={1}";
        public const string CreateOrder = "api/Order";
        public const string UpdateOrderStatus = "api/Order/{0}/status";
        public const string CancelOrder = "api/Order/{0}/cancel";
        public const string GetEstimatedWaitTime = "api/Order/{0}/wait-time";
        public const string ProcessPayment = "api/Order/{0}/payment";
        public const string GetOrderSummaries = "api/Order/summary";
        
        // Reservation endpoints
        public const string GetReservation = "api/Reservation/{0}";
        public const string GetReservationsByCustomer = "api/Reservation/customer/{0}";
        public const string GetReservationsByDate = "api/Reservation/date/{0}";
        public const string GetUpcomingReservations = "api/Reservation/upcoming";
        public const string CreateReservation = "api/Reservation";
        public const string UpdateReservation = "api/Reservation/{0}";
        public const string UpdateReservationStatus = "api/Reservation/{0}/status";
        public const string CancelReservation = "api/Reservation/{0}/cancel";
        public const string GetAvailableTables = "api/Reservation/available-tables";
        public const string CheckTableAvailability = "api/Reservation/check-table";
        
        // Notification endpoints
        public const string GetNotifications = "api/Notification";
        public const string GetUnreadNotifications = "api/Notification/unread";
        public const string GetNotification = "api/Notification/{0}";
        public const string MarkNotificationAsRead = "api/Notification/{0}/read";
        public const string MarkAllNotificationsAsRead = "api/Notification/read-all";
        public const string DeleteNotification = "api/Notification/{0}";
        public const string BroadcastNotification = "api/Notification/broadcast";
        public const string SendSystemNotification = "api/Notification/system";
        
        // User endpoints
        public const string GetAllUsers = "api/User";
        public const string GetUser = "api/User/{0}";
        public const string GetUserByUsername = "api/User/by-username/{0}";
        public const string GetUserByEmail = "api/User/by-email/{0}";
        public const string GetUsersByType = "api/User/by-type/{0}";
        public const string CreateUser = "api/User";
        public const string UpdateUser = "api/User/{0}";
        public const string UpdateUserStatus = "api/User/{0}/status";
        public const string DeleteUser = "api/User/{0}";
        
        // Report endpoints
        public const string GetDailySalesReport = "api/Report/daily/{0}";
        public const string GetWeeklySalesReport = "api/Report/weekly/{0}";
        public const string GetMonthlySalesReport = "api/Report/monthly/{0}/{1}";
        public const string GetTopSellingItems = "api/Report/top-selling?startDate={0}&endDate={1}&limit={2}";
        public const string GetSalesByCategory = "api/Report/sales-by-category?startDate={0}&endDate={1}";
        public const string GetOrdersByHour = "api/Report/orders-by-hour/{0}";
        public const string GetSalesByPaymentMethod = "api/Report/sales-by-payment?startDate={0}&endDate={1}";
        public const string ExportSalesReport = "api/Report/export/sales?startDate={0}&endDate={1}&format={2}";
        public const string GetDashboardSummary = "api/Report/dashboard-summary";
    }
}
