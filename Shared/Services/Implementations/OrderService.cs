using Shared.Constants;
using Shared.Helpers;
using Shared.Models.Common;
using Shared.Models.Order;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class OrderService : ServiceBase, IOrderService
    {
        private readonly ICacheService _cacheService;
        private readonly ISettingsService _settingsService;
        private Cart _cart;
        
        public OrderService(
            HttpClient httpClient,
            ISettingsService settingsService,
            ICacheService cacheService)
            : base(httpClient, settingsService)
        {
            _cacheService = cacheService;
            _settingsService = settingsService;
            
            // Initialize cart
            LoadCart();
        }
        
        public Cart CurrentCart => _cart;

        public async Task<Order> GetOrderByIdAsync(string orderId)
        {
            var endpoint = string.Format(ApiEndpoints.GetOrder, orderId);
            var response = await GetAsync<ApiResponse<Order>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return null;
        }

        public async Task<List<OrderSummary>> GetOrdersByCustomerIdAsync(string customerId, bool forceRefresh = false)
        {
            var cacheKey = string.Format(AppConstants.CacheUserOrders, customerId);
            
            if (!forceRefresh)
            {
                var cachedOrders = _cacheService.Get<List<OrderSummary>>(cacheKey);
                if (cachedOrders != null && cachedOrders.Count > 0)
                {
                    return cachedOrders;
                }
            }
            
            var endpoint = string.Format(ApiEndpoints.GetOrdersByCustomer, customerId);
            var response = await GetAsync<ApiResponse<List<OrderSummary>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                _cacheService.Set(cacheKey, response.Data, 
                    TimeSpan.FromMinutes(AppConstants.CacheDurationShort));
                return response.Data;
            }
            
            return new List<OrderSummary>();
        }

        public async Task<List<Order>> GetActiveOrdersAsync()
        {
            var response = await GetAsync<ApiResponse<List<Order>>>(ApiEndpoints.GetActiveOrders);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<Order>();
        }

        public async Task<List<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            var endpoint = string.Format(ApiEndpoints.GetOrdersByStatus, status);
            var response = await GetAsync<ApiResponse<List<Order>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<Order>();
        }

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var endpoint = string.Format(ApiEndpoints.GetOrdersByDateRange, 
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            
            var response = await GetAsync<ApiResponse<List<Order>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<Order>();
        }

        public async Task<string> CreateOrderAsync(CreateOrderRequest orderRequest)
        {
            var response = await PostAsync<ApiResponse<Order>>(ApiEndpoints.CreateOrder, orderRequest);
            
            if (response.Success && response.Data != null)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserOrders, userId));
                }
                
                // Clear cart
                ClearCart();
                
                return response.Data.Id;
            }
            
            return null;
        }

        public async Task<bool> UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            var endpoint = string.Format(ApiEndpoints.UpdateOrderStatus, orderId);
            var request = new { Status = status };
            
            var response = await PutAsync<ApiResponse<bool>>(endpoint, request);
            
            if (response.Success)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserOrders, userId));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<bool> CancelOrderAsync(string orderId)
        {
            var endpoint = string.Format(ApiEndpoints.CancelOrder, orderId);
            var response = await PutAsync<ApiResponse<bool>>(endpoint, new { });
            
            if (response.Success)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserOrders, userId));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<int> GetEstimatedWaitTimeAsync(string orderId)
        {
            var endpoint = string.Format(ApiEndpoints.GetEstimatedWaitTime, orderId);
            var response = await GetAsync<ApiResponse<int>>(endpoint);
            
            if (response.Success)
            {
                return response.Data;
            }
            
            return 0;
        }

        public async Task<bool> ProcessPaymentAsync(string orderId, string paymentMethod, PaymentStatus status)
        {
            var endpoint = string.Format(ApiEndpoints.ProcessPayment, orderId);
            var request = new 
            {
                PaymentMethod = paymentMethod,
                Status = status
            };
            
            var response = await PostAsync<ApiResponse<bool>>(endpoint, request);
            
            if (response.Success)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserOrders, userId));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<List<OrderSummary>> GetOrderSummariesAsync(DateTime date)
        {
            var endpoint = $"{ApiEndpoints.GetOrderSummaries}?date={date:yyyy-MM-dd}";
            var response = await GetAsync<ApiResponse<List<OrderSummary>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<OrderSummary>();
        }

        public void AddToCart(CartItem item)
        {
            _cart.AddItem(item);
            SaveCart();
        }

        public void RemoveFromCart(string menuItemId)
        {
            _cart.RemoveItem(menuItemId);
            SaveCart();
        }

        public void UpdateCartItemQuantity(string menuItemId, int quantity)
        {
            _cart.UpdateItemQuantity(menuItemId, quantity);
            SaveCart();
        }

        public void ClearCart()
        {
            _cart.Clear();
            SaveCart();
        }

        public void UpdateCartOrderType(OrderType orderType)
        {
            _cart.OrderType = orderType;
            SaveCart();
        }

        public void UpdateCartTableNumber(string tableNumber)
        {
            _cart.TableNumber = tableNumber;
            SaveCart();
        }

        public void UpdateCartTipAmount(decimal tipAmount)
        {
            _cart.TipAmount = tipAmount;
            SaveCart();
        }

        public void UpdateCartSpecialInstructions(string specialInstructions)
        {
            _cart.SpecialInstructions = specialInstructions;
            SaveCart();
        }

        public void SaveCart()
        {
            try
            {
                var cartJson = JsonHelper.Serialize(_cart);
                _settingsService.SetValue("CurrentCart", cartJson);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving cart: {ex.Message}");
            }
        }

        public void LoadCart()
        {
            try
            {
                var cartJson = _settingsService.GetValue<string>("CurrentCart");
                if (!string.IsNullOrEmpty(cartJson))
                {
                    _cart = JsonHelper.Deserialize<Cart>(cartJson);
                }
                else
                {
                    _cart = new Cart();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading cart: {ex.Message}");
                _cart = new Cart();
            }
        }

        public async Task<Order> GetCurrentOrderAsync()
        {
            var userId = _settingsService.UserId;
            if (string.IsNullOrEmpty(userId))
                return null;
            
            var orders = await GetOrdersByCustomerIdAsync(userId, true);
            var activeOrder = orders
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault(o => o.Status != OrderStatus.Completed && o.Status != OrderStatus.Cancelled);
            
            if (activeOrder != null)
            {
                return await GetOrderByIdAsync(activeOrder.Id);
            }
            
            return null;
        }

        public async Task<OrderStatus> GetOrderStatusAsync(string orderId)
        {
            var order = await GetOrderByIdAsync(orderId);
            return order?.Status ?? OrderStatus.Cancelled;
        }

        public async Task<bool> HasActiveOrderAsync()
        {
            var order = await GetCurrentOrderAsync();
            return order != null;
        }
    }
}