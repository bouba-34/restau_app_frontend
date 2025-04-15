using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Settings
{
    public partial class SettingsViewModel : ViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private readonly ICacheService _cacheService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private bool _darkModeEnabled;
        
        [ObservableProperty]
        private bool _notificationsEnabled;
        
        [ObservableProperty]
        private string _apiBaseUrl;
        
        [ObservableProperty]
        private string _username;
        
        [ObservableProperty]
        private string _userEmail;
        
        [ObservableProperty]
        private string _appVersion;
        
        [ObservableProperty]
        private ObservableCollection<string> _cacheSizes;
        
        [ObservableProperty]
        private bool _isConnected;
        
        [ObservableProperty]
        private bool _signalRConnected;
        
        public SettingsViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            ISettingsService settingsService,
            IAuthService authService,
            INotificationService notificationService,
            ICacheService cacheService,
            ISignalRService signalRService,
            IConnectivityService connectivityService) 
            : base(navigationService, dialogService)
        {
            _settingsService = settingsService;
            _authService = authService;
            _notificationService = notificationService;
            _cacheService = cacheService;
            _signalRService = signalRService;
            
            Title = "Settings";
            CacheSizes = new ObservableCollection<string>();
            
            // Subscribe to connectivity events
            connectivityService.ConnectivityChanged += (s, isConnected) => IsConnected = isConnected;
            IsConnected = connectivityService.IsConnected;
            
            // Subscribe to SignalR connection events
            _signalRService.Connected += (s, _) => SignalRConnected = true;
            _signalRService.Disconnected += (s, _) => SignalRConnected = false;
            SignalRConnected = _signalRService.IsConnected;
            
            // Load settings
            LoadSettings();
        }
        
        private void LoadSettings()
        {
            // Load app settings
            DarkModeEnabled = _settingsService.DarkMode;
            NotificationsEnabled = _settingsService.NotificationsEnabled;
            ApiBaseUrl = _settingsService.ApiBaseUrl;
            
            // Load user info
            Username = _settingsService.Username;
            UserEmail = _settingsService.UserEmail;
            
            // Set app version
            AppVersion = $"Version {AppInfo.VersionString} (Build {AppInfo.BuildString})";
            
            // Calculate cache sizes
            UpdateCacheSizes();
        }
        
        private void UpdateCacheSizes()
        {
            CacheSizes.Clear();
            CacheSizes.Add($"Menu Cache: {GetCacheSize(AppConstants.CacheMenuCategories, AppConstants.CacheMenuItems, AppConstants.CacheFeaturedItems)}");
            CacheSizes.Add($"Order Cache: {GetCacheSize($"{AppConstants.CacheUserOrders}_{_settingsService.UserId}")}");
            CacheSizes.Add($"Reservation Cache: {GetCacheSize($"{AppConstants.CacheUserReservations}_{_settingsService.UserId}")}");
        }
        
        private string GetCacheSize(params string[] cacheKeys)
        {
            int count = 0;
            
            foreach (var key in cacheKeys)
            {
                if (_cacheService.Contains(key))
                {
                    count++;
                }
            }
            
            return count > 0 ? $"{count} items" : "Empty";
        }
        
        partial void OnDarkModeEnabledChanged(bool value)
        {
            // Save dark mode setting
            _settingsService.DarkMode = value;
            _settingsService.SaveSettings();
            
            // Apply theme change
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }
        
        partial void OnNotificationsEnabledChanged(bool value)
        {
            // Save notifications setting
            _settingsService.NotificationsEnabled = value;
            _settingsService.SaveSettings();
            
            // Update notification service
            if (value)
            {
                _notificationService.StartPolling();
            }
            else
            {
                _notificationService.StopPolling();
            }
        }
        
        partial void OnApiBaseUrlChanged(string value)
        {
            // Save API URL setting if valid
            if (Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                _settingsService.ApiBaseUrl = value;
                _settingsService.SaveSettings();
            }
        }
        
        [RelayCommand]
        private async Task ClearCacheAsync()
        {
            bool confirm = await DialogService.DisplayAlertAsync(
                "Clear Cache", 
                "Are you sure you want to clear all cached data? This will not affect your login status.", 
                "Yes", 
                "No");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Clear the cache
                    _cacheService.Clear();
                    
                    // Update cache sizes
                    UpdateCacheSizes();
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync("Cache cleared successfully");
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to clear cache", ex);
                }
            }, "Clearing cache...");
        }
        
        [RelayCommand]
        private async Task ReconnectSignalRAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Disconnect first if connected
                    if (_signalRService.IsConnected)
                    {
                        await _signalRService.DisconnectAsync();
                    }
                    
                    // Try to connect
                    await _signalRService.ConnectAsync();
                    
                    // Show result
                    if (_signalRService.IsConnected)
                    {
                        await DialogService.DisplayToastAsync("Connected to server successfully");
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Connection Failed", "Failed to connect to the server. Please check your internet connection and try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to connect to server", ex);
                }
            }, "Connecting to server...");
        }
        
        [RelayCommand]
        private async Task RestoreDefaultSettingsAsync()
        {
            bool confirm = await DialogService.DisplayAlertAsync(
                "Restore Defaults", 
                "Are you sure you want to restore all settings to their default values?", 
                "Yes", 
                "No");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Restore default settings
                    _settingsService.ApiBaseUrl = AppConstants.DefaultApiBaseUrl;
                    _settingsService.DarkMode = false;
                    _settingsService.NotificationsEnabled = true;
                    _settingsService.SaveSettings();
                    
                    // Update UI
                    DarkModeEnabled = _settingsService.DarkMode;
                    NotificationsEnabled = _settingsService.NotificationsEnabled;
                    ApiBaseUrl = _settingsService.ApiBaseUrl;
                    
                    // Apply theme change
                    Application.Current.UserAppTheme = AppTheme.Light;
                    
                    // Start notification polling
                    _notificationService.StartPolling();
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync("Settings restored to defaults");
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to restore default settings", ex);
                }
            });
        }
        
        [RelayCommand]
        private async Task ViewPrivacyPolicyAsync()
        {
            await DialogService.DisplayAlertAsync(
                "Privacy Policy", 
                "This application collects and stores your information locally on your device to " +
                "provide you with a better restaurant experience. We do not share your information " +
                "with third parties without your consent. Your order history and reservation data " +
                "are stored securely and used only to provide you with the services you request.",
                "OK");
        }
        
        [RelayCommand]
        private async Task ViewAboutAsync()
        {
            await DialogService.DisplayAlertAsync(
                "About Restaurant App", 
                $"Restaurant App {AppVersion}\n\n" +
                "This application allows you to browse restaurant menus, place orders, and make reservations. " +
                "For customer support, please contact support@restaurantapp.com.\n\n" +
                "© 2025 Restaurant App. All rights reserved.",
                "OK");
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh settings
            LoadSettings();
        }
    }
}