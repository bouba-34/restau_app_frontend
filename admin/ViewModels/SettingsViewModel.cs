using admin.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using admin.Services.Interfaces;
using admin.Views;
using Shared.Constants;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;

namespace admin.ViewModels
{
    public partial class SettingsViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly ILocalSettingsService _localSettingsService;
        private readonly ISettingsService _settingsService;
        private readonly IThemeService _themeService;
        private readonly ISignalRService _signalRService;

        [ObservableProperty]
        private string apiBaseUrl;

        [ObservableProperty]
        private bool isDarkMode;

        [ObservableProperty]
        private bool notificationsEnabled;

        [ObservableProperty]
        private bool signalRConnected;

        [ObservableProperty]
        private string versionInfo;

        [ObservableProperty]
        private string userName;

        [ObservableProperty]
        private string userEmail;

        [ObservableProperty]
        private string userRole;

        public SettingsViewModel(
            IAuthService authService,
            ILocalSettingsService localSettingsService,
            ISettingsService settingsService,
            IThemeService themeService,
            ISignalRService signalRService)
        {
            Title = "Settings";
            
            _authService = authService;
            _localSettingsService = localSettingsService;
            _settingsService = settingsService;
            _themeService = themeService;
            _signalRService = signalRService;
            
            // Set version info
            VersionInfo = $"Version {AppInfo.VersionString} (Build {AppInfo.BuildString})";
            
            // Subscribe to SignalR connection state
            _signalRService.Connected += (s, e) => SignalRConnected = true;
            _signalRService.Disconnected += (s, e) => SignalRConnected = false;
            SignalRConnected = _signalRService.IsConnected;
            
            // Load settings
            LoadSettings();
        }

        public override Task InitializeAsync()
        {
            LoadSettings();
            IsInitialized = true;
            return Task.CompletedTask;
        }

        private void LoadSettings()
        {
            // Load API URL
            ApiBaseUrl = _settingsService.ApiBaseUrl ?? AppConstants.DefaultApiBaseUrl;
            
            // Load theme setting
            IsDarkMode = _themeService.IsDarkMode();
            
            // Load notifications setting
            NotificationsEnabled = _settingsService.NotificationsEnabled;
            
            // Load user info
            UserName = _settingsService.Username ?? "Unknown";
            UserEmail = _settingsService.UserEmail ?? "Unknown";
            UserRole = _settingsService.UserType ?? "Unknown";
        }

        [RelayCommand]
        private void ToggleDarkMode()
        {
            _themeService.ToggleTheme();
            IsDarkMode = _themeService.IsDarkMode();
        }

        [RelayCommand]
        private void ToggleNotifications()
        {
            _settingsService.NotificationsEnabled = NotificationsEnabled;
            
            if (NotificationsEnabled)
            {
                _signalRService.ConnectAsync();
            }
            else
            {
                _signalRService.DisconnectAsync();
            }
        }

        [RelayCommand]
        private async Task SaveApiUrlAsync()
        {
            if (string.IsNullOrWhiteSpace(ApiBaseUrl))
            {
                ApiBaseUrl = AppConstants.DefaultApiBaseUrl;
            }
            
            // Ensure URL ends with '/'
            if (!ApiBaseUrl.EndsWith("/"))
            {
                ApiBaseUrl += "/";
            }
            
            // Save settings
            _settingsService.ApiBaseUrl = ApiBaseUrl;
            
            // Ask for restart
            bool restart = await Shell.Current.DisplayAlert(
                "API URL Changed",
                "The application needs to restart to apply the new API URL. Restart now?",
                "Yes", "No");
                
            if (restart)
            {
                Application.Current.MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>());
            }
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Logout",
                "Are you sure you want to logout?",
                "Yes", "No");
                
            if (confirm)
            {
                await _authService.LogoutAsync();
                Application.Current.MainPage = new NavigationPage(ServiceHelper.GetService<LoginPage>());
            }
        }

        [RelayCommand]
        private async Task ConnectSignalRAsync()
        {
            try
            {
                ClearError();
                await _signalRService.ConnectAsync();
            }
            catch (Exception ex)
            {
                ShowError("Failed to connect to SignalR: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            // Get current password
            string currentPassword = await Shell.Current.DisplayPromptAsync(
                "Change Password",
                "Enter your current password:",
                "Next",
                "Cancel",
                placeholder: "Current password",
                maxLength: 50,
                keyboard: Keyboard.Default);
                //isPassword: true);
                
            if (string.IsNullOrEmpty(currentPassword))
                return;
                
            // Get new password
            string newPassword = await Shell.Current.DisplayPromptAsync(
                "Change Password",
                "Enter your new password:",
                "Next",
                "Cancel",
                placeholder: "New password",
                maxLength: 50,
                keyboard: Keyboard.Default);
                //isPassword: true);
                
            if (string.IsNullOrEmpty(newPassword))
                return;
                
            // Confirm new password
            string confirmPassword = await Shell.Current.DisplayPromptAsync(
                "Change Password",
                "Confirm your new password:",
                "Change",
                "Cancel",
                placeholder: "Confirm password",
                maxLength: 50,
                keyboard: Keyboard.Default);
                //isPassword: true);
                
            if (string.IsNullOrEmpty(confirmPassword))
                return;
                
            // Check if passwords match
            if (newPassword != confirmPassword)
            {
                await Shell.Current.DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }
            
            try
            {
                ClearError();
                IsBusy = true;
                
                var userId = _settingsService.UserId;
                
                if (string.IsNullOrEmpty(userId))
                {
                    await Shell.Current.DisplayAlert("Error", "User ID not found. Please log in again.", "OK");
                    return;
                }
                
                var success = await _authService.ChangePasswordAsync(userId, currentPassword, newPassword);
                
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Password changed successfully.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to change password. Please check your current password.", "OK");
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to change password: " + ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}