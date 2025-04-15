namespace Client.Constants
{
    public static class Routes
    {
        // Auth
        public const string Login = "login";
        public const string Register = "register";
        public const string Profile = "profile";

        // Menu
        public const string Menu = "menu";
        public const string MenuCategory = "menu/category";
        public const string MenuItem = "menu/item";
        public const string MenuItemDetail = "menu/item/detail";

        // Order
        public const string Cart = "cart";
        public const string Order = "order";
        public const string OrderDetail = "order/detail";
        public const string OrderHistory = "order/history";

        // Reservation
        public const string Reservation = "reservation";
        public const string ReservationDetail = "reservation/detail";
        public const string ReservationHistory = "reservation/history";

        // Notification
        public const string Notifications = "notifications";

        // Settings
        public const string Settings = "settings";
    }
}