namespace admin.Services.Interfaces
{
    public interface ILocalSettingsService
    {
        T GetSetting<T>(string key, T defaultValue = default);
        void SetSetting<T>(string key, T value);
        void RemoveSetting(string key);
        void ClearAllSettings();
    }
}