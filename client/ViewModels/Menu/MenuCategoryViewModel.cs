using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Menu;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Menu
{
    public partial class MenuCategoryViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IMenuService _menuService;
        private string _categoryId;
        
        [ObservableProperty]
        private string _categoryName;
        
        [ObservableProperty]
        private MenuCategory _category;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Menu.MenuItem> _menuItems;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _isLoading;
        
        [ObservableProperty]
        private bool _isEmpty;
        
        public MenuCategoryViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IMenuService menuService) 
            : base(navigationService, dialogService)
        {
            _menuService = menuService;
            
            Title = "Menu Category";
            MenuItems = new ObservableCollection<Shared.Models.Menu.MenuItem>();
        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("CategoryId", out var categoryId) && categoryId != null)
            {
                _categoryId = categoryId.ToString();
                
                if (query.TryGetValue("CategoryName", out var categoryName) && categoryName != null)
                {
                    CategoryName = categoryName.ToString();
                    Title = CategoryName;
                }
                
                LoadCategoryDataAsync().ConfigureAwait(false);
            }
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadCategoryDataAsync(true);
        }
        
        private async Task LoadCategoryDataAsync(bool forceRefresh = false)
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                IsLoading = true;
                
                try
                {
                    // Load category details
                    Category = await _menuService.GetCategoryByIdAsync(_categoryId);
                    
                    if (Category != null)
                    {
                        Title = Category.Name;
                        CategoryName = Category.Name;
                    }
                    
                    // Load menu items for category
                    var items = await _menuService.GetMenuItemsByCategoryAsync(_categoryId, forceRefresh);
                    
                    MenuItems.Clear();
                    foreach (var item in items)
                    {
                        MenuItems.Add(item);
                    }
                    
                    IsEmpty = MenuItems.Count == 0;
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load category data", ex);
                }
                finally
                {
                    IsRefreshing = false;
                    IsLoading = false;
                }
            });
        }
        
        [RelayCommand]
        private async Task ViewItemDetailAsync(Shared.Models.Menu.MenuItem item)
        {
            if (item == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "ItemId", item.Id }
            };
            
            await NavigationService.NavigateToAsync(Routes.MenuItemDetail, parameters);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data if needed
            if (!string.IsNullOrEmpty(_categoryId) && (Category == null || MenuItems.Count == 0))
            {
                LoadCategoryDataAsync().ConfigureAwait(false);
            }
        }
    }
}