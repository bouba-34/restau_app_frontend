
using Shared.Models.Notification;

namespace Shared.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetNotificationsAsync(bool forceRefresh = false);
        Task<List<Notification>> GetUnreadNotificationsAsync();
        Task<Notification> GetNotificationByIdAsync(string notificationId);
        Task<bool> MarkNotificationAsReadAsync(string notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync();
        Task<bool> DeleteNotificationAsync(string notificationId);
        Task<bool> SendNotificationAsync(string userId, string title, string message);
        Task<bool> BroadcastNotificationAsync(string title, string message, List<string> userIds = null);
        
        // Notification count methods
        int UnreadNotificationCount { get; }
        event EventHandler<int> UnreadCountChanged;
        Task RefreshUnreadCountAsync();
        void StartPolling();
        void StopPolling();
    }
}