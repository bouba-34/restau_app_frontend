using Shared.Constants;
using Shared.Services.Interfaces;

namespace client.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<string, object> _memoryCache = new();
        
        public SettingsService()
        {
            // Set default API URL if not already configured
            if (string.IsNullOrEmpty(ApiBaseUrl))
            {
                ApiBaseUrl = AppConstants.DefaultApiBaseUrl;
            }
            
            // Load any cached settings
            LoadSettings();
        }
        
        public string ApiBaseUrl
        {
            get => GetValue<string>(AppConstants.SettingsApiBaseUrl, AppConstants.DefaultApiBaseUrl);
            set => SetValue(AppConstants.SettingsApiBaseUrl, value);
        }
        
        public string AuthToken
        {
            get => GetValue<string>(AppConstants.SettingsAuthToken);
            set => SetValue(AppConstants.SettingsAuthToken, value);
        }
        
        public string UserId
        {
            get => GetValue<string>(AppConstants.SettingsUserId);
            set => SetValue(AppConstants.SettingsUserId, value);
        }
        
        public string Username
        {
            get => GetValue<string>(AppConstants.SettingsUsername);
            set => SetValue(AppConstants.SettingsUsername, value);
        }
        
        public string UserEmail
        {
            get => GetValue<string>(AppConstants.SettingsUserEmail);
            set => SetValue(AppConstants.SettingsUserEmail, value);
        }
        
        public string UserType
        {
            get => GetValue<string>(AppConstants.SettingsUserType);
            set => SetValue(AppConstants.SettingsUserType, value);
        }
        
        public DateTime? TokenExpiration
        {
            get => GetValue<DateTime?>(AppConstants.SettingsTokenExpiration);
            set => SetValue(AppConstants.SettingsTokenExpiration, value);
        }
        
        public bool DarkMode
        {
            get => GetValue<bool>(AppConstants.SettingsDarkMode, false);
            set => SetValue(AppConstants.SettingsDarkMode, value);
        }
        
        public bool NotificationsEnabled
        {
            get => GetValue<bool>(AppConstants.SettingsNotificationsEnabled, true);
            set => SetValue(AppConstants.SettingsNotificationsEnabled, value);
        }
        
        public bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken) && 
                                 !string.IsNullOrEmpty(UserId) && 
                                 TokenExpiration.HasValue && 
                                 TokenExpiration.Value > DateTime.UtcNow;
        
        public T GetValue<T>(string key, T defaultValue = default)
        {
            if (_memoryCache.TryGetValue(key, out var value) && value is T typedValue)
            {
                return typedValue;
            }
            
            // Try to get from secure storage
            try
            {
                string storedValue = SecureStorage.GetAsync(key).Result;
                if (!string.IsNullOrEmpty(storedValue))
                {
                    // Handle different types
                    if (typeof(T) == typeof(bool) && bool.TryParse(storedValue, out var boolResult))
                    {
                        _memoryCache[key] = boolResult;
                        return (T)(object)boolResult;
                    }
                    else if (typeof(T) == typeof(int) && int.TryParse(storedValue, out var intResult))
                    {
                        _memoryCache[key] = intResult;
                        return (T)(object)intResult;
                    }
                    else if (typeof(T) == typeof(decimal) && decimal.TryParse(storedValue, out var decimalResult))
                    {
                        _memoryCache[key] = decimalResult;
                        return (T)(object)decimalResult;
                    }
                    else if (typeof(T) == typeof(DateTime) && DateTime.TryParse(storedValue, out var dateResult))
                    {
                        _memoryCache[key] = dateResult;
                        return (T)(object)dateResult;
                    }
                    else if (typeof(T) == typeof(DateTime?))
                    {
                        if (DateTime.TryParse(storedValue, out var nullableDateResult))
                        {
                            _memoryCache[key] = (DateTime?)nullableDateResult;
                            return (T)(object)(DateTime?)nullableDateResult;
                        }
                        return defaultValue;
                    }
                    else
                    {
                        _memoryCache[key] = storedValue;
                        return (T)(object)storedValue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error retrieving setting {key}: {ex.Message}");
            }
            
            return defaultValue;
        }
        
        public void SetValue<T>(string key, T value)
        {
            _memoryCache[key] = value;
            
            try
            {
                if (value != null)
                {
                    SecureStorage.SetAsync(key, value.ToString()).Wait();
                }
                else
                {
                    SecureStorage.Remove(key);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving setting {key}: {ex.Message}");
            }
        }
        
        public void Remove(string key)
        {
            _memoryCache.Remove(key);
            
            try
            {
                SecureStorage.Remove(key);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing setting {key}: {ex.Message}");
            }
        }
        
        public void Clear()
        {
            _memoryCache.Clear();
            
            try
            {
                SecureStorage.RemoveAll();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error clearing settings: {ex.Message}");
            }
        }
        
        public void SaveSettings()
        {
            foreach (var item in _memoryCache)
            {
                try
                {
                    if (item.Value != null)
                    {
                        SecureStorage.SetAsync(item.Key, item.Value.ToString()).Wait();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error saving setting {item.Key}: {ex.Message}");
                }
            }
        }
        
        private void LoadSettings()
        {
            // Settings are loaded on-demand in GetValue<T>
        }
    }
}