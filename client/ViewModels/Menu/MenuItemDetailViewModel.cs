using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Models.Menu;
using Shared.Models.Order;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Menu
{
    public partial class MenuItemDetailViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IMenuService _menuService;
        private readonly IOrderService _orderService;
        private string _itemId;
        
        [ObservableProperty]
        private MenuItemDetail _menuItem;
        
        [ObservableProperty]
        private int _quantity = 1;
        
        [ObservableProperty]
        private ObservableCollection<string> _selectedCustomizations;
        
        [ObservableProperty]
        private string _specialInstructions;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private decimal _totalPrice;
        
        [ObservableProperty]
        private bool _hasDiscount;
        
        [ObservableProperty]
        private decimal _originalPrice;
        
        public MenuItemDetailViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IMenuService menuService,
            IOrderService orderService) 
            : base(navigationService, dialogService)
        {
            _menuService = menuService;
            _orderService = orderService;
            
            Title = "Menu Item";
            SelectedCustomizations = new ObservableCollection<string>();
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
        private void ToggleCustomization(string customization)
        {
            if (string.IsNullOrEmpty(customization))
                return;
                
            if (SelectedCustomizations.Contains(customization))
            {
                SelectedCustomizations.Remove(customization);
            }
            else
            {
                SelectedCustomizations.Add(customization);
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
                        Price = HasDiscount ? Math.Round(MenuItem.Price * (1 - (MenuItem.DiscountPercentage / 100)), 2) : MenuItem.Price,
                        Quantity = Quantity,
                        Customizations = SelectedCustomizations.ToList(),
                        SpecialInstructions = SpecialInstructions
                    };
                    
                    // Add to cart
                    _orderService.AddToCart(cartItem);
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync(Messages.ItemAddedToCart);
                    
                    // Navigate to cart
                    await NavigationService.NavigateToAsync("//CartPage");
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to add item to cart", ex);
                }
            });
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