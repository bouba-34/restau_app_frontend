using admin.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using admin.Services.Interfaces;
using admin.Views;
using Shared.Constants;
using Shared.Services.Interfaces;

namespace admin.ViewModels
{
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;
        private readonly ILocalSettingsService _localSettingsService;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private bool rememberMe;

        [ObservableProperty]
        private bool isLoggingIn;

        public LoginViewModel(
            IAuthService authService, 
            ILocalSettingsService localSettingsService,
            ISettingsService settingsService)
        {
            Title = "Login";
            _authService = authService;
            _localSettingsService = localSettingsService;
            _settingsService = settingsService;

            // Load remembered credentials if available
            RememberMe = _localSettingsService.GetSetting<bool>("RememberMe", false);
            if (RememberMe)
            {
                Username = _localSettingsService.GetSetting<string>("Username", string.Empty);
                // Note: We don't store actual password for security reasons
                // Only username is remembered if remember me is checked
            }

            // Set default API URL if not set
            if (string.IsNullOrEmpty(_settingsService.ApiBaseUrl))
            {
                _settingsService.ApiBaseUrl = AppConstants.DefaultApiBaseUrl;
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ShowError("Username and password are required.");
                return;
            }

            try
            {
                IsLoggingIn = true;
                ClearError();

                // Attempt login
                var response = await _authService.LoginAsync(Username, Password);

                // Save remember me preference
                _localSettingsService.SetSetting("RememberMe", RememberMe);
                if (RememberMe)
                {
                    _localSettingsService.SetSetting("Username", Username);
                }
                else
                {
                    _localSettingsService.RemoveSetting("Username");
                }

                // Set logged in flag
                _localSettingsService.SetSetting("IsLoggedIn", true);

                // Check if user has admin role
                if (response.UserType != Shared.Models.Auth.UserType.Admin && 
                    response.UserType != Shared.Models.Auth.UserType.Staff)
                {
                    ShowError("You don't have permission to access the admin panel. Only Admin and Staff accounts are allowed.");
                    await _authService.LogoutAsync();
                    _localSettingsService.SetSetting("IsLoggedIn", false);
                    return;
                }

                // Navigate to main app
                var autService = ServiceHelper.GetService<IAuthService>();
                var settingsService = ServiceHelper.GetService<ISettingsService>();
                Application.Current.MainPage = new AppShell(autService, settingsService);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }
}