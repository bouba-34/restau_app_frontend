using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Menu;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    public partial class MenuManagementViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;

        [ObservableProperty]
        private ObservableCollection<Shared.Models.Menu.MenuItem> menuItems = new();

        [ObservableProperty]
        private ObservableCollection<MenuCategory> categories = new();

        [ObservableProperty]
        private MenuCategory selectedCategory;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private bool isLoadingItems;

        [ObservableProperty]
        private bool isLoadingCategories;

        [ObservableProperty]
        private bool showFeaturedOnly;

        [ObservableProperty]
        private bool showAllItems = true;

        public MenuManagementViewModel(IMenuService menuService)
        {
            Title = "Menu Management";
            _menuService = menuService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await Task.WhenAll(
                LoadCategoriesAsync(),
                LoadMenuItemsAsync()
            );
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await Task.WhenAll(
                LoadCategoriesAsync(true),
                LoadMenuItemsAsync(true)
            );
            IsRefreshing = false;
        }

        private async Task LoadCategoriesAsync(bool forceRefresh = false)
        {
            try
            {
                IsLoadingCategories = true;
                ClearError();

                var categoryList = await _menuService.GetCategoriesAsync(forceRefresh);
                Categories.Clear();
                
                // Add "All Categories" option
                Categories.Add(new MenuCategory { Id = "", Name = "All Categories" });
                
                foreach (var category in categoryList)
                {
                    Categories.Add(category);
                }
                
                // Set default selection to "All Categories"
                if (SelectedCategory == null)
                {
                    SelectedCategory = Categories.First();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load categories: " + ex.Message);
            }
            finally
            {
                IsLoadingCategories = false;
            }
        }

        private async Task LoadMenuItemsAsync(bool forceRefresh = false)
        {
            try
            {
                IsLoadingItems = true;
                ClearError();

                List<Shared.Models.Menu.MenuItem> items;

                if (ShowFeaturedOnly)
                {
                    // Load featured items
                    items = await _menuService.GetFeaturedMenuItemsAsync(forceRefresh);
                }
                else if (SelectedCategory != null && !string.IsNullOrEmpty(SelectedCategory.Id))
                {
                    // Load items for selected category
                    items = await _menuService.GetMenuItemsByCategoryAsync(SelectedCategory.Id, forceRefresh);
                }
                else
                {
                    // Load all items
                    items = await _menuService.GetMenuItemsAsync(forceRefresh);
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    items = items.Where(i => 
                        i.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        i.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                MenuItems.Clear();
                foreach (var item in items)
                {
                    MenuItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load menu items: " + ex.Message);
            }
            finally
            {
                IsLoadingItems = false;
            }
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            await LoadMenuItemsAsync(true);
        }

        [RelayCommand]
        private async Task ViewMenuItemDetailsAsync(string menuItemId)
        {
            if (string.IsNullOrEmpty(menuItemId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "MenuItemId", menuItemId }
            };

            await Shell.Current.GoToAsync($"MenuItemDetailPage", parameters);
        }

        [RelayCommand]
        private async Task AddNewMenuItemAsync()
        {
            await Shell.Current.GoToAsync($"MenuItemDetailPage?IsNew=true");
        }

        [RelayCommand]
        private async Task AddNewCategoryAsync()
        {
            // For simplicity, we're implementing a basic dialog
            string categoryName = await Shell.Current.DisplayPromptAsync(
                "New Category", 
                "Enter category name:",
                "Create",
                "Cancel",
                "Category name");

            if (string.IsNullOrWhiteSpace(categoryName))
                return;

            try
            {
                IsLoadingCategories = true;
                ClearError();

                var newCategory = new MenuCategory
                {
                    Name = categoryName,
                    Description = "",
                    IsActive = true
                };

                var category = await _menuService.CreateCategoryAsync(newCategory);
                if (category != null)
                {
                    await LoadCategoriesAsync(true);
                    SelectedCategory = Categories.FirstOrDefault(c => c.Id == category.Id);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to create category: " + ex.Message);
            }
            finally
            {
                IsLoadingCategories = false;
            }
        }

        [RelayCommand]
        private async Task ToggleItemAvailabilityAsync(string menuItemId)
        {
            var menuItem = MenuItems.FirstOrDefault(i => i.Id == menuItemId);
            if (menuItem == null)
                return;

            try
            {
                IsLoadingItems = true;
                ClearError();

                var success = await _menuService.UpdateMenuItemAvailabilityAsync(menuItemId, !menuItem.IsAvailable);
                if (success)
                {
                    // Update the item in the collection
                    menuItem.IsAvailable = !menuItem.IsAvailable;
                    
                    // Refresh the collection to update UI
                    int index = MenuItems.IndexOf(menuItem);
                    if (index >= 0)
                    {
                        MenuItems.RemoveAt(index);
                        MenuItems.Insert(index, menuItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update item availability: " + ex.Message);
            }
            finally
            {
                IsLoadingItems = false;
            }
        }

        partial void OnSelectedCategoryChanged(MenuCategory value)
        {
            if (IsInitialized)
            {
                LoadMenuItemsAsync().ConfigureAwait(false);
            }
        }

        partial void OnShowFeaturedOnlyChanged(bool value)
        {
            if (IsInitialized)
            {
                LoadMenuItemsAsync().ConfigureAwait(false);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (IsInitialized && !string.IsNullOrEmpty(value) && value.Length > 2)
            {
                LoadMenuItemsAsync().ConfigureAwait(false);
            }
            else if (IsInitialized && string.IsNullOrEmpty(value))
            {
                LoadMenuItemsAsync().ConfigureAwait(false);
            }
        }
    }
}