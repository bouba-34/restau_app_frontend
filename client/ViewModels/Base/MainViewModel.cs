using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;

namespace Client.ViewModels.Base
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly ISettingsService _settingsService;
        private readonly INavigationService _navigationService;
        private readonly IDialogService _dialogService;
        private readonly IConnectivityService _connectivityService;
        private readonly INotificationService _notificationService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private bool _isLoggedIn;
        
        [ObservableProperty]
        private bool _isNotLoggedIn;
        
        [ObservableProperty]
        private bool _isConnected;
        
        [ObservableProperty]
        private bool _isBusy;
        
        [ObservableProperty]
        private string _title;
        
        [ObservableProperty]
        private int _unreadNotificationCount;
        
        public MainViewModel(
            IAuthService authService,
            ISettingsService settingsService,
            INavigationService navigationService,
            IDialogService dialogService,
            IConnectivityService connectivityService,
            INotificationService notificationService,
            ISignalRService signalRService)
        {
            _authService = authService;
            _settingsService = settingsService;
            _navigationService = navigationService;
            _dialogService = dialogService;
            _connectivityService = connectivityService;
            _notificationService = notificationService;
            _signalRService = signalRService;
            
            // Set initial values
            UpdateLoginState();
            IsConnected = _connectivityService.IsConnected;
            Title = "Restaurant App";
            
            // Subscribe to events
            _connectivityService.ConnectivityChanged += OnConnectivityChanged;
            _notificationService.UnreadCountChanged += OnUnreadNotificationCountChanged;
            
            // Start services
            _connectivityService.StartMonitoring();
            
            // Initialize SignalR if logged in
            if (IsLoggedIn)
            {
                InitializeSignalR();
            }
        }
        
        private void UpdateLoginState()
        {
            IsLoggedIn = _authService.HasValidSession();
            IsNotLoggedIn = !IsLoggedIn;
        }
        
        private void OnConnectivityChanged(object sender, bool isConnected)
        {
            IsConnected = isConnected;
            
            if (isConnected && IsLoggedIn)
            {
                // Reconnect SignalR if needed
                if (!_signalRService.IsConnected)
                {
                    InitializeSignalR();
                }
            }
        }
        
        private void OnUnreadNotificationCountChanged(object sender, int count)
        {
            UnreadNotificationCount = count;
        }
        
        private async void InitializeSignalR()
        {
            if (!_signalRService.IsConnected && IsConnected)
            {
                await _signalRService.ConnectAsync();
            }
        }
        
        [RelayCommand]
        private async Task Logout()
        {
            if (IsBusy) return;
            
            try
            {
                IsBusy = true;
                
                bool confirm = await _dialogService.DisplayAlertAsync(
                    "Logout", 
                    "Are you sure you want to logout?", 
                    "Yes", 
                    "No");
                    
                if (!confirm) return;
                
                // Disconnect SignalR
                await _signalRService.DisconnectAsync();
                
                // Logout and redirect to login page
                await _authService.LogoutAsync();
                
                // Update login state
                UpdateLoginState();
                
                // Navigate to login page
                await _navigationService.NavigateToAsync("//LoginPage");
                
                await _dialogService.DisplayToastAsync("You have been logged out");
            }
            finally
            {
                IsBusy = false;
            }
        }
        
        public void OnAppearing()
        {
            // Refresh authentication state
            UpdateLoginState();
            
            // Initialize SignalR if logged in and not already connected
            if (IsLoggedIn && !_signalRService.IsConnected && IsConnected)
            {
                InitializeSignalR();
            }
            
            // Refresh unread notification count if logged in
            if (IsLoggedIn)
            {
                _notificationService.RefreshUnreadCountAsync();
            }
        }
        
        public void OnDisappearing()
        {
            // Nothing to do here for now
        }
    }
}