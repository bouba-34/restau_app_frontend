namespace admin.Services.Interfaces
{
    public interface IThemeService
    {
        void SetTheme(bool isDarkMode);
        bool IsDarkMode();
        void ToggleTheme();
    }
}