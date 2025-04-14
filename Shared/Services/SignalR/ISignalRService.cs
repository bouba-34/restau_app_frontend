using Shared.Models.Order;
using Shared.Models.Reservation;

namespace Shared.Services.SignalR
{
    public interface ISignalRService
    {
        bool IsConnected { get; }
        Task ConnectAsync();
        Task DisconnectAsync();
        
        // Events
        event EventHandler<string> Connected;
        event EventHandler<Exception> ConnectionFailed;
        event EventHandler Disconnected;
        
        // Order events
        event EventHandler<(string OrderId, OrderStatus Status)> OrderStatusChanged;
        event EventHandler<string> NewOrder;
        
        // Reservation events
        event EventHandler<(string ReservationId, ReservationStatus Status)> ReservationStatusChanged;
        event EventHandler<string> NewReservation;
        
        // Menu events
        event EventHandler<(string MenuItemId, bool IsAvailable)> MenuItemAvailabilityChanged;
        
        // Notification events
        event EventHandler<(string Title, string Message)> NotificationReceived;
    }
}