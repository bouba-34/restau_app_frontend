namespace Shared.Services.Interfaces
{
    public interface ISettingsService
    {
        string ApiBaseUrl { get; set; }
        string AuthToken { get; set; }
        string UserId { get; set; }
        string Username { get; set; }
        string UserEmail { get; set; }
        string UserType { get; set; }
        DateTime? TokenExpiration { get; set; }
        bool IsLoggedIn { get; }
        bool DarkMode { get; set; }
        bool NotificationsEnabled { get; set; }
        
        T GetValue<T>(string key, T defaultValue = default);
        void SetValue<T>(string key, T value);
        void Remove(string key);
        void Clear();
        void SaveSettings();
    }
}