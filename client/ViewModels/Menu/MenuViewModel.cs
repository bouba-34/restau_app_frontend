using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Menu;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Menu
{
    public partial class MenuViewModel : ViewModelBase
    {
        private readonly IMenuService _menuService;
        
        [ObservableProperty]
        private ObservableCollection<MenuCategory> _categories;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Menu.MenuItem> _featuredItems;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private string _searchQuery;
        
        [ObservableProperty]
        private bool _isSearching;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Menu.MenuItem> _searchResults;
        
        public MenuViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IMenuService menuService) 
            : base(navigationService, dialogService)
        {
            _menuService = menuService;
            
            Title = "Menu";
            Categories = new ObservableCollection<MenuCategory>();
            FeaturedItems = new ObservableCollection<Shared.Models.Menu.MenuItem>();
            SearchResults = new ObservableCollection<Shared.Models.Menu.MenuItem>();
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await RefreshDataAsync();
        }
        
        [RelayCommand]
        private async Task RefreshDataAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    // Load categories
                    var categories = await _menuService.GetCategoriesAsync(true);
                    Categories.Clear();
                    foreach (var category in categories.OrderBy(c => c.DisplayOrder))
                    {
                        Categories.Add(category);
                    }
                    
                    // Load featured items
                    var featuredItems = await _menuService.GetFeaturedMenuItemsAsync(true);
                    FeaturedItems.Clear();
                    foreach (var item in featuredItems)
                    {
                        FeaturedItems.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load menu data", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        [RelayCommand]
        private async Task ViewCategoryAsync(MenuCategory category)
        {
            if (category == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "CategoryId", category.Id },
                { "CategoryName", category.Name }
            };
            
            await NavigationService.NavigateToAsync(Routes.MenuCategory, parameters);
        }
        
        [RelayCommand]
        private async Task ViewItemAsync(Shared.Models.Menu.MenuItem item)
        {
            if (item == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "ItemId", item.Id }
            };
            
            await NavigationService.NavigateToAsync(Routes.MenuItemDetail, parameters);
        }
        
        [RelayCommand]
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                IsSearching = false;
                SearchResults.Clear();
                return;
            }
            
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    var results = await _menuService.SearchMenuItemsAsync(SearchQuery);
                    
                    SearchResults.Clear();
                    foreach (var item in results)
                    {
                        SearchResults.Add(item);
                    }
                    
                    IsSearching = true;
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Search failed", ex);
                }
            });
        }
        
        [RelayCommand]
        private void ClearSearch()
        {
            SearchQuery = string.Empty;
            IsSearching = false;
            SearchResults.Clear();
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data if needed
            if (Categories.Count == 0 || FeaturedItems.Count == 0)
            {
                RefreshDataAsync().ConfigureAwait(false);
            }
        }
    }
}