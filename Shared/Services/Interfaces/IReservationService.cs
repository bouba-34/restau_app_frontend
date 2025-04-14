using Shared.Models.Reservation;

namespace Shared.Services.Interfaces
{
    public interface IReservationService
    {
        Task<Reservation> GetReservationByIdAsync(string reservationId);
        Task<List<Reservation>> GetReservationsByCustomerIdAsync(string customerId, bool forceRefresh = false);
        Task<List<Reservation>> GetReservationsByDateAsync(DateTime date);
        Task<List<Reservation>> GetUpcomingReservationsAsync();
        Task<string> CreateReservationAsync(CreateReservationRequest request);
        Task<Reservation> UpdateReservationAsync(string reservationId, CreateReservationRequest request);
        Task<bool> UpdateReservationStatusAsync(string reservationId, ReservationStatus status);
        Task<bool> CancelReservationAsync(string reservationId);
        Task<List<AvailableTable>> GetAvailableTablesAsync(DateTime date, TimeSpan time, int partySize);
        Task<bool> IsTableAvailableAsync(string tableNumber, DateTime date, TimeSpan time);
        
        // Reservation tracking methods
        Task<Reservation> GetCurrentReservationAsync();
        Task<ReservationStatus> GetReservationStatusAsync(string reservationId);
        Task<bool> HasActiveReservationAsync();
    }
}