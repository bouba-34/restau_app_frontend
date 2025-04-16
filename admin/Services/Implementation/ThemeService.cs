using admin.Services.Interfaces;

namespace admin.Services.Implementation
{
    public class ThemeService : IThemeService
    {
        private readonly ILocalSettingsService _localSettingsService;

        public ThemeService(ILocalSettingsService localSettingsService)
        {
            _localSettingsService = localSettingsService;
        }

        public bool IsDarkMode()
        {
            return _localSettingsService.GetSetting<bool>("DarkMode", false);
        }

        public void SetTheme(bool isDarkMode)
        {
            // Save the setting
            _localSettingsService.SetSetting("DarkMode", isDarkMode);

            // Apply the theme
            if (isDarkMode)
            {
                Application.Current.UserAppTheme = AppTheme.Dark;
            }
            else
            {
                Application.Current.UserAppTheme = AppTheme.Light;
            }
        }

        public void ToggleTheme()
        {
            var isDarkMode = IsDarkMode();
            SetTheme(!isDarkMode);
        }
    }
}