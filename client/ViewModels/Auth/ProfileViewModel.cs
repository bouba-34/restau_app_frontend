using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Helpers;
using Shared.Models.Auth;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Auth
{
    public partial class ProfileViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly ISettingsService _settingsService;
        
        [ObservableProperty]
        private UserProfile _userProfile;
        
        [ObservableProperty]
        private string _currentPassword;
        
        [ObservableProperty]
        private string _newPassword;
        
        [ObservableProperty]
        private string _confirmPassword;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private bool _isPasswordChangeMode;
        
        [ObservableProperty]
        private ObservableCollection<string> _validationErrors;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        public ProfileViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IAuthService authService,
            ISettingsService settingsService) 
            : base(navigationService, dialogService)
        {
            _authService = authService;
            _settingsService = settingsService;
            
            Title = "My Profile";
            ValidationErrors = new ObservableCollection<string>();
            
            // Initialize with default profile
            UserProfile = new UserProfile
            {
                Id = _settingsService.UserId,
                Username = _settingsService.Username,
                Email = _settingsService.UserEmail
            };
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await LoadProfileAsync();
        }
        
        [RelayCommand]
        private async Task LoadProfileAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    UserProfile = await _authService.GetProfileAsync();
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load profile", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            
            if (!IsEditMode)
            {
                // Reset validation errors when exiting edit mode
                ValidationErrors.Clear();
            }
        }
        
        [RelayCommand]
        private void TogglePasswordChangeMode()
        {
            IsPasswordChangeMode = !IsPasswordChangeMode;
            
            if (!IsPasswordChangeMode)
            {
                // Clear password fields and validation errors
                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;
                ValidationErrors.Clear();
            }
        }
        
        [RelayCommand]
        private async Task SaveProfileAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                ValidationErrors.Clear();
                
                // Validate email
                if (!ValidationHelper.IsValidEmail(UserProfile.Email))
                {
                    ValidationErrors.Add("Invalid email format");
                    return;
                }
                
                // Validate phone number if provided
                if (!string.IsNullOrEmpty(UserProfile.PhoneNumber) && !ValidationHelper.IsValidPhone(UserProfile.PhoneNumber))
                {
                    ValidationErrors.Add("Invalid phone number format");
                    return;
                }
                
                try
                {
                    bool success = await _authService.UpdateProfileAsync(UserProfile);
                    
                    if (success)
                    {
                        // Update stored email
                        _settingsService.UserEmail = UserProfile.Email;
                        _settingsService.SaveSettings();
                        
                        // Exit edit mode
                        IsEditMode = false;
                        
                        // Show success message
                        await DialogService.DisplayToastAsync(Messages.ProfileUpdated);
                    }
                    else
                    {
                        ValidationErrors.Add("Failed to update profile");
                    }
                }
                catch (Exception ex)
                {
                    ValidationErrors.Add(ex.Message);
                }
            }, "Updating profile...");
        }
        
        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                ValidationErrors.Clear();
                
                // Validate current password
                if (string.IsNullOrEmpty(CurrentPassword))
                {
                    ValidationErrors.Add("Current password is required");
                    return;
                }
                
                // Validate new password
                if (!ValidationHelper.IsValidPassword(NewPassword))
                {
                    ValidationErrors.Add("Password must contain at least 6 characters, including uppercase, lowercase, and numbers");
                    return;
                }
                
                // Validate password confirmation
                if (!ValidationHelper.PasswordsMatch(NewPassword, ConfirmPassword))
                {
                    ValidationErrors.Add("Passwords do not match");
                    return;
                }
                
                try
                {
                    bool success = await _authService.ChangePasswordAsync(
                        UserProfile.Id, CurrentPassword, NewPassword);
                    
                    if (success)
                    {
                        // Exit password change mode
                        IsPasswordChangeMode = false;
                        
                        // Clear password fields
                        CurrentPassword = string.Empty;
                        NewPassword = string.Empty;
                        ConfirmPassword = string.Empty;
                        
                        // Show success message
                        await DialogService.DisplayToastAsync(Messages.PasswordChanged);
                    }
                    else
                    {
                        ValidationErrors.Add("Failed to change password");
                    }
                }
                catch (Exception ex)
                {
                    ValidationErrors.Add(ex.Message);
                }
            }, "Changing password...");
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data
            LoadProfileAsync().ConfigureAwait(false);
        }
    }
}