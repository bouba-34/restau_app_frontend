namespace admin.Helpers
{
    public static class ThemeHelper
    {
        public static void SetTheme(bool isDarkMode)
        {
            Application.Current.UserAppTheme = isDarkMode ? AppTheme.Dark : AppTheme.Light;
        }

        public static bool IsDarkTheme()
        {
            return Application.Current.UserAppTheme == AppTheme.Dark;
        }
    }
}