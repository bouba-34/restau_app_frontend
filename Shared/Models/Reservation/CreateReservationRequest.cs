namespace Shared.Models.Reservation
{
    public class CreateReservationRequest
    {
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public int PartySize { get; set; }
        public string SpecialRequests { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
    }
}