namespace Shared.Models.Reservation
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }
    
    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved,
        Unavailable
    }
}