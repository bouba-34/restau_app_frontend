namespace Shared.Models.Order
{
    public class OrderSummary
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public OrderType Type { get; set; }
        public decimal TotalAmount { get; set; }
        public int ItemCount { get; set; }
        public string CustomerName { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}