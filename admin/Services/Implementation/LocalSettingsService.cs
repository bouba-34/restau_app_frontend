using admin.Services.Interfaces;
using System.Text.Json;

namespace admin.Services.Implementation
{
    public class LocalSettingsService : ILocalSettingsService
    {
        public T GetSetting<T>(string key, T defaultValue = default)
        {
            if (Preferences.ContainsKey(key))
            {
                string json = Preferences.Get(key, null);
                if (string.IsNullOrEmpty(json))
                    return defaultValue;

                try
                {
                    return JsonSerializer.Deserialize<T>(json);
                }
                catch
                {
                    // If deserialization fails, return default value
                    return defaultValue;
                }
            }

            return defaultValue;
        }

        public void SetSetting<T>(string key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            Preferences.Set(key, json);
        }

        public void RemoveSetting(string key)
        {
            if (Preferences.ContainsKey(key))
            {
                Preferences.Remove(key);
            }
        }

        public void ClearAllSettings()
        {
            Preferences.Clear();
        }
    }
}