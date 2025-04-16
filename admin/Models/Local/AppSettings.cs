namespace admin.Models.Local
{
    public class AppSettings
    {
        public string ApiBaseUrl { get; set; } = string.Empty;
        public bool DarkMode { get; set; } = false;
        public bool NotificationsEnabled { get; set; } = true;
        public bool IsLoggedIn { get; set; } = false;
    }
}