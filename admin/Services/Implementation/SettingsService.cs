using admin.Services.Interfaces;
using Shared.Constants;
using Shared.Services.Interfaces;

namespace admin.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly ILocalSettingsService _localSettingsService;

        public SettingsService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public string ApiBaseUrl
        {
            get => _localSettingsService.GetSetting<string>("ApiBaseUrl", AppConstants.DefaultApiBaseUrl);
            set => _localSettingsService.SetSetting("ApiBaseUrl", value);
        }

        public string AuthToken
        {
            get => _localSettingsService.GetSetting<string>("AuthToken", string.Empty);
            set => _localSettingsService.SetSetting("AuthToken", value);
        }

        public string UserId
        {
            get => _localSettingsService.GetSetting<string>("UserId", string.Empty);
            set => _localSettingsService.SetSetting("UserId", value);
        }

        public string Username
        {
            get => _localSettingsService.GetSetting<string>("Username", string.Empty);
            set => _localSettingsService.SetSetting("Username", value);
        }

        public string UserEmail
        {
            get => _localSettingsService.GetSetting<string>("UserEmail", string.Empty);
            set => _localSettingsService.SetSetting("UserEmail", value);
        }

        public string UserType
        {
            get => _localSettingsService.GetSetting<string>("UserType", string.Empty);
            set => _localSettingsService.SetSetting("UserType", value);
        }

        public DateTime? TokenExpiration
        {
            get => _localSettingsService.GetSetting<DateTime?>("TokenExpiration", null);
            set => _localSettingsService.SetSetting("TokenExpiration", value);
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken) && !string.IsNullOrEmpty(UserId);

        public bool DarkMode
        {
            get => _localSettingsService.GetSetting<bool>("DarkMode", false);
            set => _localSettingsService.SetSetting("DarkMode", value);
        }

        public bool NotificationsEnabled
        {
            get => _localSettingsService.GetSetting<bool>("NotificationsEnabled", true);
            set => _localSettingsService.SetSetting("NotificationsEnabled", value);
        }

        public T GetValue<T>(string key, T defaultValue = default)
        {
            return _localSettingsService.GetSetting<T>(key, defaultValue);
        }

        public void SetValue<T>(string key, T value)
        {
            _localSettingsService.SetSetting(key, value);
        }

        public void Remove(string key)
        {
            _localSettingsService.RemoveSetting(key);
        }

        public void Clear()
        {
            _localSettingsService.ClearAllSettings();
        }

        public void SaveSettings()
        {
            // The local settings service handles saving already
        }
    }
}