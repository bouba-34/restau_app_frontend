using Shared.Models.Order;

namespace Shared.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> GetOrderByIdAsync(string orderId);
        Task<List<OrderSummary>> GetOrdersByCustomerIdAsync(string customerId, bool forceRefresh = false);
        Task<List<Order>> GetActiveOrdersAsync();
        Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<string> CreateOrderAsync(CreateOrderRequest orderRequest);
        Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status);
        Task<bool> CancelOrderAsync(string orderId);
        Task<int> GetEstimatedWaitTimeAsync(string orderId);
        Task<bool> ProcessPaymentAsync(string orderId, string paymentMethod, PaymentStatus status);
        Task<List<OrderSummary>> GetOrderSummariesAsync(DateTime date);
        
        // Cart management methods
        Cart CurrentCart { get; }
        void AddToCart(CartItem item);
        void RemoveFromCart(string menuItemId);
        void UpdateCartItemQuantity(string menuItemId, int quantity);
        void ClearCart();
        void UpdateCartOrderType(OrderType orderType);
        void UpdateCartTableNumber(string tableNumber);
        void UpdateCartTipAmount(decimal tipAmount);
        void UpdateCartSpecialInstructions(string specialInstructions);
        void SaveCart();
        void LoadCart();
        
        // Order tracking methods
        Task<Order> GetCurrentOrderAsync();
        Task<OrderStatus> GetOrderStatusAsync(string orderId);
        Task<bool> HasActiveOrderAsync();
    }
}