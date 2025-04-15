using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Notification;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Notification
{
    public partial class NotificationViewModel : ViewModelBase
    {
        private readonly INotificationService _notificationService;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Notification.Notification> _notifications;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Notification.Notification> _unreadNotifications;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _hasNotifications;
        
        [ObservableProperty]
        private bool _hasUnreadNotifications;
        
        [ObservableProperty]
        private bool _isLoading;
        
        [ObservableProperty]
        private NotificationType _selectedFilter = NotificationType.System;
        
        [ObservableProperty]
        private bool _isFilteringEnabled;
        
        public NotificationViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            INotificationService notificationService) 
            : base(navigationService, dialogService)
        {
            _notificationService = notificationService;
            
            Title = "Notifications";
            Notifications = new ObservableCollection<Shared.Models.Notification.Notification>();
            UnreadNotifications = new ObservableCollection<Shared.Models.Notification.Notification>();
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await LoadNotificationsAsync();
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadNotificationsAsync(true);
        }
        
        [RelayCommand]
        private async Task MarkAllAsReadAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    bool success = await _notificationService.MarkAllNotificationsAsReadAsync();
                    
                    if (success)
                    {
                        // Update all notifications to read
                        foreach (var notification in Notifications)
                        {
                            notification.IsRead = true;
                        }
                        
                        // Clear unread notifications
                        UnreadNotifications.Clear();
                        HasUnreadNotifications = false;
                        
                        // Show confirmation
                        await DialogService.DisplayToastAsync("All notifications marked as read");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to mark notifications as read", ex);
                }
            });
        }
        
        [RelayCommand]
        private async Task MarkAsReadAsync(Shared.Models.Notification.Notification notification)
        {
            if (notification == null || notification.IsRead)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    bool success = await _notificationService.MarkNotificationAsReadAsync(notification.Id);
                    
                    if (success)
                    {
                        // Update notification status
                        notification.IsRead = true;
                        
                        // Remove from unread list
                        UnreadNotifications.Remove(notification);
                        HasUnreadNotifications = UnreadNotifications.Count > 0;
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to mark notification as read", ex);
                }
            });
        }
        
        [RelayCommand]
        private async Task DeleteNotificationAsync(Shared.Models.Notification.Notification notification)
        {
            if (notification == null)
                return;
                
            bool confirm = await DialogService.DisplayAlertAsync(
                "Delete Notification", 
                "Are you sure you want to delete this notification?", 
                "Delete", 
                "Cancel");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    bool success = await _notificationService.DeleteNotificationAsync(notification.Id);
                    
                    if (success)
                    {
                        // Remove from lists
                        Notifications.Remove(notification);
                        
                        if (!notification.IsRead)
                        {
                            UnreadNotifications.Remove(notification);
                            HasUnreadNotifications = UnreadNotifications.Count > 0;
                        }
                        
                        HasNotifications = Notifications.Count > 0;
                        
                        // Show confirmation
                        await DialogService.DisplayToastAsync("Notification deleted");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to delete notification", ex);
                }
            });
        }
        
        [RelayCommand]
        private void ToggleFiltering()
        {
            IsFilteringEnabled = !IsFilteringEnabled;
            
            // Refresh the view based on filter
            ApplyFilterIfNeeded();
        }
        
        partial void OnSelectedFilterChanged(NotificationType value)
        {
            if (IsFilteringEnabled)
            {
                ApplyFilterIfNeeded();
            }
        }
        
        private void ApplyFilterIfNeeded()
        {
            if (IsFilteringEnabled)
            {
                // Apply filter
                LoadFilteredNotifications();
            }
            else
            {
                // Reset to full list
                LoadNotificationsAsync().ConfigureAwait(false);
            }
        }
        
        private void LoadFilteredNotifications()
        {
            // Load all notifications but filter them
            var allNotifications = _notificationService.GetNotificationsAsync().Result;
            
            // Filter by selected type
            var filteredNotifications = allNotifications.Where(n => n.Type == SelectedFilter).ToList();
            
            Notifications.Clear();
            foreach (var notification in filteredNotifications.OrderByDescending(n => n.CreatedAt))
            {
                Notifications.Add(notification);
            }
            
            HasNotifications = Notifications.Count > 0;
            
            // Update unread notifications
            var unreadFiltered = filteredNotifications.Where(n => !n.IsRead).ToList();
            
            UnreadNotifications.Clear();
            foreach (var notification in unreadFiltered.OrderByDescending(n => n.CreatedAt))
            {
                UnreadNotifications.Add(notification);
            }
            
            HasUnreadNotifications = UnreadNotifications.Count > 0;
        }
        
        private async Task LoadNotificationsAsync(bool forceRefresh = false)
        {
            IsLoading = true;
            IsRefreshing = true;
            
            try
            {
                // Load all notifications
                var allNotifications = await _notificationService.GetNotificationsAsync(forceRefresh);
                
                Notifications.Clear();
                foreach (var notification in allNotifications.OrderByDescending(n => n.CreatedAt))
                {
                    Notifications.Add(notification);
                }
                
                HasNotifications = Notifications.Count > 0;
                
                // Load unread notifications
                var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync();
                
                UnreadNotifications.Clear();
                foreach (var notification in unreadNotifications.OrderByDescending(n => n.CreatedAt))
                {
                    UnreadNotifications.Add(notification);
                }
                
                HasUnreadNotifications = UnreadNotifications.Count > 0;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load notifications", ex);
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh notifications
            LoadNotificationsAsync().ConfigureAwait(false);
        }
    }
}