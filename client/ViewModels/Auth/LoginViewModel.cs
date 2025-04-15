using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Helpers;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Auth
{
    public partial class LoginViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly ISettingsService _settingsService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private string _username;
        
        [ObservableProperty]
        private string _password;
        
        [ObservableProperty]
        private bool _rememberMe;
        
        [ObservableProperty]
        private ObservableCollection<string> _validationErrors;
        
        public LoginViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IAuthService authService,
            ISettingsService settingsService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _authService = authService;
            _settingsService = settingsService;
            _signalRService = signalRService;
            
            Title = "Login";
            ValidationErrors = new ObservableCollection<string>();
            
            // Check if we have saved credentials
            LoadSavedCredentials();
        }
        
        private void LoadSavedCredentials()
        {
            var savedUsername = _settingsService.GetValue<string>("SavedUsername");
            if (!string.IsNullOrEmpty(savedUsername))
            {
                Username = savedUsername;
                RememberMe = true;
            }
        }
        
        private void SaveCredentials()
        {
            if (RememberMe)
            {
                _settingsService.SetValue("SavedUsername", Username);
            }
            else
            {
                _settingsService.Remove("SavedUsername");
            }
        }
        
        [RelayCommand]
        private async Task LoginAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                // Clear validation errors
                ValidationErrors.Clear();
                
                // Validate input
                var errors = ValidationHelper.ValidateLoginRequest(Username, Password);
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ValidationErrors.Add(error);
                    }
                    return;
                }
                
                try
                {
                    // Attempt login
                    var authResponse = await _authService.LoginAsync(Username, Password);
                    //Console.WriteLine($"data from log viewmodel {authResponse.Token}");
                    // Save credentials if remember me is checked
                    SaveCredentials();
                    
                    // Initialize SignalR
                    if (_settingsService.IsLoggedIn)
                    {
                        try
                        {
                            await _signalRService.ConnectAsync();
                            Console.WriteLine("signalR connected");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"SignalR connection failed: {ex.Message}");
                            // We continue anyway, as this is not critical
                        }
                    }
                    
                    // Navigate to main page
                    await NavigationService.NavigateToAsync(Routes.Menu);
                    
                    // Show welcome message
                    await DialogService.DisplayToastAsync($"Welcome, {authResponse.Username}!");
                }
                catch (UnauthorizedAccessException)
                {
                    ValidationErrors.Add(Messages.LoginFailed);
                }
                catch (Exception ex)
                {
                    ValidationErrors.Add(ex.Message);
                }
            }, "Logging in...");
        }
        
        [RelayCommand]
        private async Task NavigateToRegisterAsync()
        {
            //Console.WriteLine("going to register page...");
            await NavigationService.NavigateToAsync(Routes.Register);
        }
        
        [RelayCommand]
        private async Task ForgotPasswordAsync()
        {
            await DialogService.DisplayAlertAsync(
                "Reset Password", 
                "Please contact customer support to reset your password.", 
                "OK");
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Clear password field for security
            Password = string.Empty;
            
            // Check if we have a valid session
            if (_authService.HasValidSession())
            {
                // Navigate to main page
                NavigationService.NavigateToAsync(Routes.Menu);
            }
        }
    }
}