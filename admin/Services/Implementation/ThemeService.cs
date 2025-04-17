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

        /*public void SetTheme(bool isDarkMode)
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
        }*/
        
        // In ThemeService.cs
        public void SetTheme(bool isDarkMode)
        {
            // Set the theme in preferences
            _localSettingsService.SetSetting("DarkMode", isDarkMode);
    
            // Apply the theme
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
    
            // Manually update resources if needed
            /*if (isDarkMode)
            {
                Application.Current.Resources["AppBackgroundColor"] = Application.Current.Resources["DarkBackgroundColor"];
                Application.Current.Resources["CardBackgroundColor"] = Application.Current.Resources["DarkCardColor"];
                Application.Current.Resources["TextPrimaryColor"] = Application.Current.Resources["DarkTextColor"];
                Application.Current.Resources["TextSecondaryColor"] = Color.FromHex("#AAAAAA");
                Application.Current.Resources["BorderColor"] = Application.Current.Resources["DarkBorderColor"];
                Application.Current.Resources["SeparatorColor"] = Application.Current.Resources["DarkBorderColor"];
            }
            else
            {
                Application.Current.Resources["AppBackgroundColor"] = Application.Current.Resources["LightBackgroundColor"];
                Application.Current.Resources["CardBackgroundColor"] = Application.Current.Resources["LightCardColor"];
                Application.Current.Resources["TextPrimaryColor"] = Application.Current.Resources["LightTextColor"];
                Application.Current.Resources["TextSecondaryColor"] = Color.FromHex("#757575");
                Application.Current.Resources["BorderColor"] = Application.Current.Resources["LightBorderColor"];
                Application.Current.Resources["SeparatorColor"] = Application.Current.Resources["LightBorderColor"];
            }*/
        }

        public void ToggleTheme()
        {
            var isDarkMode = IsDarkMode();
            SetTheme(!isDarkMode);
        }
    }
}