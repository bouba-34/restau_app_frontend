namespace Shared.Models.Order
{
    public class CreateOrderItemRequest
    {
        public string MenuItemId { get; set; }
        public int Quantity { get; set; }
        public List<string> Customizations { get; set; } = new List<string>();
        public string SpecialInstructions { get; set; }
    }

    public class CreateOrderRequest
    {
        public List<CreateOrderItemRequest> Items { get; set; } = new List<CreateOrderItemRequest>();
        public OrderType Type { get; set; }
        public string TableNumber { get; set; }
        public string SpecialInstructions { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TipAmount { get; set; }
    }
}