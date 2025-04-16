namespace admin.Models.Local
{
    public class UserSettings
    {
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
        public DateTime? TokenExpiration { get; set; }
        public bool RememberMe { get; set; } = false;
    }
}