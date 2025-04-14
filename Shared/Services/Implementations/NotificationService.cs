using Shared.Constants;
using Shared.Models.Common;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;
using Notification = Shared.Models.Notification.Notification;

namespace Shared.Services.Implementations
{
    public class NotificationService : ServiceBase, INotificationService
    {
        private readonly ICacheService _cacheService;
        private readonly ISettingsService _settingsService;
        private readonly Timer _pollingTimer;
        private int _unreadCount = 0;

        public event EventHandler<int> UnreadCountChanged;

        public int UnreadNotificationCount => _unreadCount;

        public NotificationService(
            HttpClient httpClient,
            ISettingsService settingsService,
            ICacheService cacheService)
            : base(httpClient, settingsService)
        {
            _cacheService = cacheService;
            _settingsService = settingsService;

            // Setup polling timer
            _pollingTimer = new Timer(async _ => await RefreshUnreadCountAsync(),
                null, Timeout.Infinite, Timeout.Infinite);

            // If notifications are enabled, start polling
            if (_settingsService.NotificationsEnabled)
            {
                StartPolling();
            }
        }

        public async Task<List<Notification>> GetNotificationsAsync(bool forceRefresh = false)
        {
            var cacheKey = "UserNotifications";

            if (!forceRefresh)
            {
                var cachedNotifications = _cacheService.Get<List<Notification>>(cacheKey);
                if (cachedNotifications != null && cachedNotifications.Count > 0)
                {
                    return cachedNotifications;
                }
            }

            try
            {
                var response = await GetAsync<ApiResponse<List<Notification>>>(ApiEndpoints.GetNotifications);

                if (response.Success && response.Data != null)
                {
                    _cacheService.Set(cacheKey, response.Data,
                        TimeSpan.FromMinutes(AppConstants.CacheDurationShort));
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting notifications: {ex.Message}");
            }

            return new List<Notification>();
        }
        
        public async Task<List<Notification>> GetUnreadNotificationsAsync()
        {
            try
            {
                var response = await GetAsync<ApiResponse<List<Notification>>>(ApiEndpoints.GetUnreadNotifications);
                
                if (response.Success && response.Data != null)
                {
                    _unreadCount = response.Data.Count;
                    UnreadCountChanged?.Invoke(this, _unreadCount);
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting unread notifications: {ex.Message}");
            }
            
            return new List<Notification>();
        }

        public async Task<Notification> GetNotificationByIdAsync(string notificationId)
        {
            var endpoint = string.Format(ApiEndpoints.GetNotification, notificationId);
            
            try
            {
                var response = await GetAsync<ApiResponse<Notification>>(endpoint);
                
                if (response.Success && response.Data != null)
                {
                    return response.Data;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting notification: {ex.Message}");
            }
            
            return null;
        }

        public async Task<bool> MarkNotificationAsReadAsync(string notificationId)
        {
            var endpoint = string.Format(ApiEndpoints.MarkNotificationAsRead, notificationId);
            
            try
            {
                var response = await PutAsync<ApiResponse<bool>>(endpoint, new { });
                
                if (response.Success)
                {
                    // Clear cache
                    _cacheService.Remove("UserNotifications");
                    
                    // Refresh unread count
                    await RefreshUnreadCountAsync();
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking notification as read: {ex.Message}");
            }
            
            return false;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync()
        {
            try
            {
                var response = await PutAsync<ApiResponse<bool>>(ApiEndpoints.MarkAllNotificationsAsRead, new { });
                
                if (response.Success)
                {
                    // Clear cache
                    _cacheService.Remove("UserNotifications");
                    
                    // Update unread count
                    _unreadCount = 0;
                    UnreadCountChanged?.Invoke(this, _unreadCount);
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error marking all notifications as read: {ex.Message}");
            }
            
            return false;
        }

        public async Task<bool> DeleteNotificationAsync(string notificationId)
        {
            var endpoint = string.Format(ApiEndpoints.DeleteNotification, notificationId);
            
            try
            {
                var response = await DeleteAsync<ApiResponse<bool>>(endpoint);
                
                if (response.Success)
                {
                    // Clear cache
                    _cacheService.Remove("UserNotifications");
                    
                    // Refresh unread count
                    await RefreshUnreadCountAsync();
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting notification: {ex.Message}");
            }
            
            return false;
        }

        public async Task<bool> SendNotificationAsync(string userId, string title, string message)
        {
            var request = new
            {
                UserId = userId,
                Title = title,
                Message = message
            };
            
            try
            {
                var response = await PostAsync<ApiResponse<bool>>(ApiEndpoints.SendSystemNotification, request);
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending notification: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> BroadcastNotificationAsync(string title, string message, List<string> userIds = null)
        {
            var request = new
            {
                Title = title,
                Message = message,
                UserIds = userIds
            };
            
            try
            {
                var response = await PostAsync<ApiResponse<bool>>(ApiEndpoints.BroadcastNotification, request);
                return response.Success;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error broadcasting notification: {ex.Message}");
                return false;
            }
        }

        public async Task RefreshUnreadCountAsync()
        {
            if (!_settingsService.IsLoggedIn || !_settingsService.NotificationsEnabled)
            {
                _unreadCount = 0;
                UnreadCountChanged?.Invoke(this, _unreadCount);
                return;
            }
            
            try
            {
                var unreadNotifications = await GetUnreadNotificationsAsync();
                _unreadCount = unreadNotifications.Count;
                UnreadCountChanged?.Invoke(this, _unreadCount);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error refreshing unread count: {ex.Message}");
            }
        }

        public void StartPolling()
        {
            if (_settingsService.NotificationsEnabled)
            {
                // Start polling immediately and then every N seconds
                _pollingTimer.Change(0, AppConstants.NotificationRefreshInterval * 1000);
            }
        }

        public void StopPolling()
        {
            _pollingTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

    }
}