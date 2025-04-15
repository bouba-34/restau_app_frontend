using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Helpers;
using Shared.Models.Auth;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Auth
{
    public partial class RegisterViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        
        [ObservableProperty]
        private string _username;
        
        [ObservableProperty]
        private string _email;
        
        [ObservableProperty]
        private string _phoneNumber;
        
        [ObservableProperty]
        private string _password;
        
        [ObservableProperty]
        private string _confirmPassword;
        
        [ObservableProperty]
        private bool _termsAccepted;
        
        [ObservableProperty]
        private ObservableCollection<string> _validationErrors;
        
        public RegisterViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IAuthService authService) 
            : base(navigationService, dialogService)
        {
            _authService = authService;
            
            Title = "Register";
            ValidationErrors = new ObservableCollection<string>();
        }
        
        [RelayCommand]
        private async Task RegisterAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                // Clear validation errors
                ValidationErrors.Clear();
                
                // Validate input
                var errors = ValidationHelper.ValidateRegisterRequest(Username, Email, PhoneNumber, Password, ConfirmPassword);
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ValidationErrors.Add(error);
                    }
                    return;
                }
                
                // Validate terms acceptance
                if (!TermsAccepted)
                {
                    ValidationErrors.Add("You must accept the terms and conditions.");
                    return;
                }
                
                try
                {
                    // Create register request
                    var registerRequest = new RegisterRequest
                    {
                        Username = Username,
                        Email = Email,
                        PhoneNumber = PhoneNumber,
                        Password = Password,
                        ConfirmPassword = ConfirmPassword
                    };
                    
                    // Attempt registration
                    var authResponse = await _authService.RegisterAsync(registerRequest);
                    
                    // Navigate to main page
                    await NavigationService.NavigateToAsync(Routes.Menu);
                    
                    // Show success message
                    await DialogService.DisplayToastAsync(Messages.RegisterSuccess);
                }
                catch (Exception ex)
                {
                    ValidationErrors.Add(ex.Message);
                }
            }, "Creating your account...");
        }
        
        [RelayCommand]
        private async Task NavigateToLoginAsync()
        {
            await NavigationService.NavigateToAsync("login");
        }
        
        [RelayCommand]
        private async Task ViewTermsAndConditionsAsync()
        {
            await DialogService.DisplayAlertAsync(
                "Terms and Conditions", 
                "By creating an account, you agree to our restaurant's terms of service and privacy policy. " +
                "We will collect and process your data in accordance with our privacy policy.", 
                "OK");
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Clear password fields for security
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            
            // Clear validation errors
            ValidationErrors.Clear();
            
            // Check if we have a valid session
            if (_authService.HasValidSession())
            {
                // Navigate to main page
                NavigationService.NavigateToAsync(Routes.Menu);
            }
        }
    }
}