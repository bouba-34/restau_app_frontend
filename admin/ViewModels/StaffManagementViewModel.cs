using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Auth;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    public partial class StaffManagementViewModel : BaseViewModel
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private ObservableCollection<UserProfile> staffMembers = new();

        [ObservableProperty]
        private UserType selectedUserType = UserType.Staff;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private ObservableCollection<UserType> userTypeOptions = new(new[] { UserType.Staff, UserType.Admin });

        [ObservableProperty]
        private bool isLoading;

        public StaffManagementViewModel(IUserService userService)
        {
            Title = "Staff Management";
            _userService = userService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await RefreshAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await LoadStaffMembersAsync();
            IsRefreshing = false;
        }

        private async Task LoadStaffMembersAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                List<UserProfile> usersList;

                // Get users by type
                usersList = await _userService.GetUsersByTypeAsync(SelectedUserType);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    usersList = usersList.Where(u => 
                        u.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        u.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                StaffMembers.Clear();
                foreach (var user in usersList)
                {
                    StaffMembers.Add(user);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load staff members: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            await LoadStaffMembersAsync();
        }

        [RelayCommand]
        private async Task EditStaffAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "UserId", userId }
            };

            await Shell.Current.GoToAsync($"StaffDetailPage", parameters);
        }

        [RelayCommand]
        private async Task AddStaffAsync()
        {
            await Shell.Current.GoToAsync($"StaffDetailPage?IsNew=true");
        }

        partial void OnSelectedUserTypeChanged(UserType value)
        {
            if (IsInitialized)
            {
                LoadStaffMembersAsync().ConfigureAwait(false);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (IsInitialized && !string.IsNullOrEmpty(value) && value.Length > 2)
            {
                LoadStaffMembersAsync().ConfigureAwait(false);
            }
            else if (IsInitialized && string.IsNullOrEmpty(value))
            {
                LoadStaffMembersAsync().ConfigureAwait(false);
            }
        }
    }
}