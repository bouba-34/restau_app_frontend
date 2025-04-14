using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Order
{
    public partial class CartViewModel : ViewModelBase
    {
        private readonly IOrderService _orderService;
        
        [ObservableProperty]
        private ObservableCollection<CartItem> _items;
        
        [ObservableProperty]
        private decimal _subtotal;
        
        [ObservableProperty]
        private decimal _tax;
        
        [ObservableProperty]
        private decimal _tipAmount;
        
        [ObservableProperty]
        private decimal _totalAmount;
        
        [ObservableProperty]
        private string _specialInstructions;
        
        [ObservableProperty]
        private ObservableCollection<string> _tableNumbers;
        
        [ObservableProperty]
        private string _selectedTableNumber;
        
        [ObservableProperty]
        private OrderType _selectedOrderType;
        
        [ObservableProperty]
        private bool _isTableNumberRequired;
        
        [ObservableProperty]
        private int _itemCount;
        
        [ObservableProperty]
        private bool _isEmptyCart;
        
        [ObservableProperty]
        private decimal _tipPercentage = AppConstants.DefaultTipPercentage;
        
        [ObservableProperty]
        private ObservableCollection<decimal> _tipPercentageOptions;
        
        public CartViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IOrderService orderService) 
            : base(navigationService, dialogService)
        {
            _orderService = orderService;
            
            Title = "Cart";
            Items = new ObservableCollection<CartItem>();
            TableNumbers = new ObservableCollection<string>();
            TipPercentageOptions = new ObservableCollection<decimal> { 0, 0.1m, 0.15m, 0.18m, 0.2m, 0.25m };
            
            // Add mock table numbers (1-20)
            for (int i = 1; i <= 20; i++)
            {
                TableNumbers.Add(i.ToString());
            }
        }
        
        public override Task InitializeAsync(object parameter = null)
        {
            LoadCart();
            return Task.CompletedTask;
        }
        
        private void LoadCart()
        {
            // Load the cart from the service
            _orderService.LoadCart();
            var cart = _orderService.CurrentCart;
            
            // Update UI
            Items.Clear();
            foreach (var item in cart.Items)
            {
                Items.Add(item);
            }
            
            SpecialInstructions = cart.SpecialInstructions;
            SelectedOrderType = cart.OrderType;
            SelectedTableNumber = cart.TableNumber;
            
            // Set tip amount based on cart subtotal and default percentage
            if (cart.Subtotal > 0)
            {
                TipAmount = Math.Round(cart.Subtotal * TipPercentage, 2);
            }
            else
            {
                TipAmount = 0;
            }
            
            UpdateTotals();
            UpdateCartState();
        }
        
        private void UpdateTotals()
        {
            // Calculate subtotal
            Subtotal = Items.Sum(i => i.Subtotal);
            
            // Calculate tax (10%)
            Tax = Math.Round(Subtotal * AppConstants.TaxRate, 2);
            
            // Calculate total
            TotalAmount = Subtotal + Tax + TipAmount;
            
            // Update cart
            _orderService.UpdateCartTipAmount(TipAmount);
            _orderService.UpdateCartSpecialInstructions(SpecialInstructions);
            _orderService.SaveCart();
        }
        
        private void UpdateCartState()
        {
            ItemCount = Items.Sum(i => i.Quantity);
            IsEmptyCart = ItemCount == 0;
            IsTableNumberRequired = SelectedOrderType == OrderType.DineIn;
        }
        
        partial void OnSelectedOrderTypeChanged(OrderType value)
        {
            IsTableNumberRequired = value == OrderType.DineIn;
            
            // Update cart
            _orderService.UpdateCartOrderType(value);
            _orderService.SaveCart();
        }
        
        partial void OnSelectedTableNumberChanged(string value)
        {
            // Update cart
            _orderService.UpdateCartTableNumber(value);
            _orderService.SaveCart();
        }
        
        partial void OnSpecialInstructionsChanged(string value)
        {
            // Update cart
            _orderService.UpdateCartSpecialInstructions(value);
            _orderService.SaveCart();
        }
        
        partial void OnTipPercentageChanged(decimal value)
        {
            // Calculate tip amount
            TipAmount = Math.Round(Subtotal * value, 2);
            
            // Update totals
            UpdateTotals();
        }
        
        partial void OnTipAmountChanged(decimal value)
        {
            // Update tip percentage if subtotal > 0
            if (Subtotal > 0)
            {
                TipPercentage = Math.Round(value / Subtotal, 2);
            }
            
            // Update totals
            UpdateTotals();
        }
        
        [RelayCommand]
        private void RemoveItem(CartItem item)
        {
            if (item == null)
                return;
                
            _orderService.RemoveFromCart(item.MenuItemId);
            
            // Update UI
            Items.Remove(item);
            UpdateTotals();
            UpdateCartState();
        }
        
        [RelayCommand]
        private void IncreaseQuantity(CartItem item)
        {
            if (item == null)
                return;
                
            if (item.Quantity < 99)
            {
                _orderService.UpdateCartItemQuantity(item.MenuItemId, item.Quantity + 1);
                
                // Update UI (refresh the collection)
                LoadCart();
            }
        }
        
        [RelayCommand]
        private void DecreaseQuantity(CartItem item)
        {
            if (item == null)
                return;
                
            if (item.Quantity > 1)
            {
                _orderService.UpdateCartItemQuantity(item.MenuItemId, item.Quantity - 1);
                
                // Update UI (refresh the collection)
                LoadCart();
            }
            else
            {
                // Remove item if quantity is 1
                RemoveItem(item);
            }
        }
        
        [RelayCommand]
        private void ClearCart()
        {
            _orderService.ClearCart();
            
            // Update UI
            Items.Clear();
            UpdateTotals();
            UpdateCartState();
        }
        
        [RelayCommand]
        private async Task CheckoutAsync()
        {
            if (IsEmptyCart)
            {
                await DialogService.DisplayToastAsync(Messages.EmptyCart);
                return;
            }
            
            if (IsTableNumberRequired && string.IsNullOrEmpty(SelectedTableNumber))
            {
                await DialogService.DisplayAlertAsync("Missing Information", "Please select a table number for dine-in orders.", "OK");
                return;
            }
            
            // Confirm order
            bool confirm = await DialogService.DisplayAlertAsync(
                Messages.ConfirmPlaceOrder,
                $"Total: {TotalAmount:C}\nItems: {ItemCount}\nOrder Type: {SelectedOrderType}",
                "Place Order",
                "Cancel");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Create order request
                    var orderRequest = new CreateOrderRequest
                    {
                        Type = SelectedOrderType,
                        TableNumber = SelectedTableNumber,
                        SpecialInstructions = SpecialInstructions,
                        TipAmount = TipAmount,
                        PaymentMethod = "CreditCard", // Default payment method
                        Items = Items.Select(i => new CreateOrderItemRequest
                        {
                            MenuItemId = i.MenuItemId,
                            Quantity = i.Quantity,
                            Customizations = i.Customizations,
                            SpecialInstructions = i.SpecialInstructions
                        }).ToList()
                    };
                    
                    // Place order
                    var orderId = await _orderService.CreateOrderAsync(orderRequest);
                    
                    if (string.IsNullOrEmpty(orderId))
                    {
                        await DialogService.DisplayAlertAsync("Order Failed", "Failed to place order. Please try again.", "OK");
                        return;
                    }
                    
                    // Clear cart (should already be cleared by the service)
                    LoadCart();
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync(Messages.OrderPlaced);
                    
                    // Navigate to order detail page
                    var parameters = new Dictionary<string, object>
                    {
                        { "OrderId", orderId }
                    };
                    
                    await NavigationService.NavigateToAsync("order/detail", parameters);
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to place order", ex);
                }
            }, Messages.ProcessingRequest);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh cart
            LoadCart();
        }
    }
}