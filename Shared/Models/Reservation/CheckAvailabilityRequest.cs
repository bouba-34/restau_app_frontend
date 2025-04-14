namespace Shared.Models.Reservation
{
    public class CheckAvailabilityRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int PartySize { get; set; }
    }
}