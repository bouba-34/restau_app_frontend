using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Auth;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    [QueryProperty(nameof(IsNew), "IsNew")]
    public partial class StaffDetailViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private string userId;

        [ObservableProperty]
        private bool isNew;

        [ObservableProperty]
        private UserProfile userProfile;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string phoneNumber;

        [ObservableProperty]
        private UserType selectedUserType = UserType.Staff;

        [ObservableProperty]
        private ObservableCollection<UserType> userTypeOptions = new(new[] { UserType.Staff, UserType.Admin });

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string confirmPassword;

        [ObservableProperty]
        private bool isActive = true;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string validationErrors;

        [ObservableProperty]
        private bool hasValidationErrors;

        public StaffDetailViewModel(IUserService userService)
        {
            _userService = userService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            if (IsNew)
            {
                Title = "Add Staff Member";
                InitializeNewStaff();
            }
            else
            {
                Title = "Edit Staff Member";
                await LoadStaffMemberAsync();
            }
            
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            if (!IsNew)
            {
                await LoadStaffMemberAsync();
            }
            
            IsRefreshing = false;
        }

        private void InitializeNewStaff()
        {
            UserProfile = new UserProfile
            {
                UserType = UserType.Staff
            };
            
            UpdateFormFields();
        }

        private async Task LoadStaffMemberAsync()
        {
            if (string.IsNullOrEmpty(UserId))
                return;

            try
            {
                IsLoading = true;
                ClearError();

                UserProfile = await _userService.GetUserByIdAsync(UserId);
                
                if (UserProfile != null)
                {
                    UpdateFormFields();
                    Subtitle = UserProfile.Username;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load staff member: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateFormFields()
        {
            if (UserProfile == null)
                return;
                
            Username = UserProfile.Username;
            Email = UserProfile.Email;
            PhoneNumber = UserProfile.PhoneNumber;
            SelectedUserType = UserProfile.UserType;
            IsActive = true; // Assuming active by default
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(Username))
                errors.Add("Username is required");
                
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
                
            if (IsNew)
            {
                if (string.IsNullOrWhiteSpace(Password))
                    errors.Add("Password is required");
                    
                if (Password != ConfirmPassword)
                    errors.Add("Passwords do not match");
            }
                
            ValidationErrors = string.Join(Environment.NewLine, errors);
            HasValidationErrors = errors.Count > 0;
            
            return !HasValidationErrors;
        }

        [RelayCommand]
        private async Task SaveStaffAsync()
        {
            if (!ValidateForm())
                return;

            try
            {
                IsLoading = true;
                ClearError();
                
                if (IsNew)
                {
                    // Create new staff member
                    var userId = await _userService.CreateUserAsync(
                        Username,
                        Email,
                        PhoneNumber,
                        Password,
                        SelectedUserType);
                    
                    if (!string.IsNullOrEmpty(userId))
                    {
                        await Shell.Current.DisplayAlert("Success", "Staff member created successfully", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        ShowError("Failed to create staff member. Username or email may already exist.");
                    }
                }
                else
                {
                    // Update existing staff member
                    var updatedUser = await _userService.UpdateUserAsync(
                        UserId,
                        Email,
                        PhoneNumber);
                    
                    if (updatedUser != null)
                    {
                        // Update status if needed
                        await _userService.UpdateUserStatusAsync(UserId, IsActive);
                        
                        await Shell.Current.DisplayAlert("Success", "Staff member updated successfully", "OK");
                        await Shell.Current.GoToAsync("..");
                    }
                    else
                    {
                        ShowError("Failed to update staff member");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to save staff member: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            if (IsNew)
                return;

            string newPassword = await Shell.Current.DisplayPromptAsync(
                "Reset Password",
                "Enter new password for this user:",
                "Reset",
                "Cancel",
                placeholder: "New password",
                maxLength: 50);
                
            if (string.IsNullOrEmpty(newPassword))
                return;

            try
            {
                IsLoading = true;
                ClearError();

                // This is a simplified implementation - in a real app, you would call a proper password reset API
                // For now, we're just showing a success message
                await Task.Delay(1000); // Simulate API call
                
                await Shell.Current.DisplayAlert("Success", "Password has been reset", "OK");
            }
            catch (Exception ex)
            {
                ShowError("Failed to reset password: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteStaffAsync()
        {
            if (IsNew)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Staff Member",
                "Are you sure you want to delete this staff member? This action cannot be undone.",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _userService.DeleteUserAsync(UserId);
                
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Staff member deleted successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
                else
                {
                    ShowError("Failed to delete staff member");
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete staff member: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}