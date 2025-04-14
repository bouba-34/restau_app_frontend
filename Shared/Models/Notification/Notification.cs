namespace Shared.Models.Notification
{
    public class Notification
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
    }
    
    public enum NotificationType
    {
        OrderStatus,
        Reservation,
        Promotion,
        System
    }
}