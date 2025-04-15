using Client.Constants;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Menu;
using Shared.Models.Order;
using Shared.Services.Interfaces;

namespace Client.ViewModels.Menu
{
    public partial class MenuItemViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private string _itemId;
        
        [ObservableProperty]
        private Shared.Models.Menu.MenuItem _menuItem;
        
        [ObservableProperty]
        private int _quantity = 1;
        
        [ObservableProperty]
        private decimal _totalPrice;
        
        [ObservableProperty]
        private bool _hasDiscount;
        
        [ObservableProperty]
        private decimal _originalPrice;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        public MenuItemViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IMenuService menuService,
            IOrderService orderService) 
            : base(navigationService, dialogService)
        {
            _menuService = menuService;
            _orderService = orderService;
            
            Title = "Menu Item";
        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ItemId", out var itemId))
            {
                _itemId = itemId.ToString();
                LoadItemAsync().ConfigureAwait(false);
            }
        }
        
        private async Task LoadItemAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    // Load item details
                    var item = await _menuService.GetMenuItemDetailAsync(_itemId);
                    MenuItem = item;
                    
                    // Set title
                    Title = item.Name;
                    
                    // Calculate prices
                    UpdatePrices();
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load item details", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        private void UpdatePrices()
        {
            if (MenuItem == null)
                return;
                
            // Set price with discount if applicable
            if (MenuItem.DiscountPercentage > 0)
            {
                HasDiscount = true;
                OriginalPrice = MenuItem.Price;
                decimal discountedPrice = MenuItem.Price * (1 - (MenuItem.DiscountPercentage / 100));
                TotalPrice = Math.Round(discountedPrice * Quantity, 2);
            }
            else
            {
                HasDiscount = false;
                OriginalPrice = 0;
                TotalPrice = MenuItem.Price * Quantity;
            }
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadItemAsync();
        }
        
        [RelayCommand]
        private void AddQuantity()
        {
            if (Quantity < 99)
            {
                Quantity++;
                UpdatePrices();
            }
        }
        
        [RelayCommand]
        private void RemoveQuantity()
        {
            if (Quantity > 1)
            {
                Quantity--;
                UpdatePrices();
            }
        }
        
        [RelayCommand]
        private async Task AddToCartAsync()
        {
            if (MenuItem == null)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    // Create cart item
                    var cartItem = new CartItem
                    {
                        MenuItemId = MenuItem.Id,
                        Name = MenuItem.Name,
                        ImageUrl = MenuItem.ImageUrl,
                        Price = HasDiscount ? 
                            Math.Round(MenuItem.Price * (1 - (MenuItem.DiscountPercentage / 100)), 2) : 
                            MenuItem.Price,
                        Quantity = Quantity
                    };
                    
                    // Add to cart
                    _orderService.AddToCart(cartItem);
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync("Item added to cart");
                    
                    // Navigate to cart or stay on page
                    bool goToCart = await DialogService.DisplayAlertAsync(
                        "Added to Cart", 
                        "Item has been added to your cart", 
                        "Go to Cart", 
                        "Continue Shopping");
                        
                    if (goToCart)
                    {
                        await NavigationService.NavigateToAsync(Routes.Cart);
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to add item to cart", ex);
                }
            });
        }
        
        [RelayCommand]
        private async Task ViewDetailAsync()
        {
            if (MenuItem == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "ItemId", MenuItem.Id }
            };
            
            await NavigationService.NavigateToAsync(Routes.MenuItemDetail, parameters);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data if needed
            if (MenuItem == null && !string.IsNullOrEmpty(_itemId))
            {
                LoadItemAsync().ConfigureAwait(false);
            }
        }
    }
}