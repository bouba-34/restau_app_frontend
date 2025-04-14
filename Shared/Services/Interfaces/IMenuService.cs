using Shared.Models.Menu;

namespace Shared.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuCategory>> GetCategoriesAsync(bool forceRefresh = false);
        Task<MenuCategory> GetCategoryByIdAsync(string categoryId);
        Task<MenuCategory> CreateCategoryAsync(MenuCategory category, Stream imageStream = null);
        Task<MenuCategory> UpdateCategoryAsync(MenuCategory category, Stream imageStream = null);
        Task<bool> DeleteCategoryAsync(string categoryId);
        
        Task<List<MenuItem>> GetMenuItemsAsync(bool forceRefresh = false);
        Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string categoryId, bool forceRefresh = false);
        Task<List<MenuItem>> GetFeaturedMenuItemsAsync(bool forceRefresh = false);
        Task<MenuItemDetail> GetMenuItemDetailAsync(string menuItemId);
        Task<List<MenuItem>> SearchMenuItemsAsync(string query);
        Task<MenuItemDetail> CreateMenuItemAsync(MenuItemDetail menuItem, Stream imageStream = null);
        Task<MenuItemDetail> UpdateMenuItemAsync(MenuItemDetail menuItem, Stream imageStream = null);
        Task<bool> UpdateMenuItemAvailabilityAsync(string menuItemId, bool isAvailable);
        Task<bool> DeleteMenuItemAsync(string menuItemId);
    }
}