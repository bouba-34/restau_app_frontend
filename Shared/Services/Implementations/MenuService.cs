using Shared.Constants;
using Shared.Constants;
using Shared.Helpers;
using Shared.Models.Common;
using Shared.Models.Menu;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class MenuService : ServiceBase, IMenuService
    {
        private readonly ICacheService _cacheService;

        public MenuService(
            HttpClient httpClient,
            ISettingsService settingsService,
            ICacheService cacheService)
            : base(httpClient, settingsService)
        {
            _cacheService = cacheService;
        }

        public async Task<List<MenuCategory>> GetCategoriesAsync(bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                var cachedCategories = _cacheService.Get<List<MenuCategory>>(AppConstants.CacheMenuCategories);
                if (cachedCategories != null && cachedCategories.Count > 0)
                {
                    return cachedCategories;
                }
            }

            var response = await GetAsync<ApiResponse<List<MenuCategory>>>(ApiEndpoints.GetCategories);

            if (response.Success && response.Data != null)
            {
                _cacheService.Set(AppConstants.CacheMenuCategories, response.Data,
                    TimeSpan.FromMinutes(AppConstants.CacheDurationMedium));
                return response.Data;
            }

            return new List<MenuCategory>();
        }

        public async Task<MenuCategory> GetCategoryByIdAsync(string categoryId)
        {
            var endpoint = string.Format(ApiEndpoints.GetCategory, categoryId);
            var response = await GetAsync<ApiResponse<MenuCategory>>(endpoint);

            if (response.Success && response.Data != null)
            {
                return response.Data;
            }

            return null;
        }

        public async Task<MenuCategory> CreateCategoryAsync(MenuCategory category, Stream imageStream = null)
        {
            if (imageStream != null)
            {
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var formData = await ImageHelper.CreateImageContentAsync(imageStream, fileName);

                // Add other fields
                formData.Add(new StringContent(category.Name), "Name");
                formData.Add(new StringContent(category.Description ?? string.Empty), "Description");
                formData.Add(new StringContent(category.DisplayOrder.ToString()), "DisplayOrder");
                formData.Add(new StringContent(category.IsActive.ToString()), "IsActive");

                var response =
                    await PostFormDataAsync<ApiResponse<MenuCategory>>(ApiEndpoints.CreateCategory, formData);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuCategories);
                    return response.Data;
                }
            }
            else
            {
                var response = await PostAsync<ApiResponse<MenuCategory>>(ApiEndpoints.CreateCategory, category);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuCategories);
                    return response.Data;
                }
            }

            return null;
        }

        public async Task<MenuCategory> UpdateCategoryAsync(MenuCategory category, Stream imageStream = null)
        {
            var endpoint = string.Format(ApiEndpoints.UpdateCategory, category.Id);

            if (imageStream != null)
            {
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var formData = await ImageHelper.CreateImageContentAsync(imageStream, fileName);

                // Add other fields
                formData.Add(new StringContent(category.Name), "Name");
                formData.Add(new StringContent(category.Description ?? string.Empty), "Description");
                formData.Add(new StringContent(category.DisplayOrder.ToString()), "DisplayOrder");
                formData.Add(new StringContent(category.IsActive.ToString()), "IsActive");

                var response = await PutFormDataAsync<ApiResponse<MenuCategory>>(endpoint, formData);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuCategories);
                    return response.Data;
                }
            }
            else
            {
                var response = await PutAsync<ApiResponse<MenuCategory>>(endpoint, category);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuCategories);
                    return response.Data;
                }
            }

            return null;
        }

        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            var endpoint = string.Format(ApiEndpoints.DeleteCategory, categoryId);
            var response = await DeleteAsync<ApiResponse<bool>>(endpoint);

            if (response.Success)
            {
                // Clear cache
                _cacheService.Remove(AppConstants.CacheMenuCategories);
                return true;
            }

            return false;
        }

        public async Task<List<MenuItem>> GetMenuItemsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                var cachedItems = _cacheService.Get<List<MenuItem>>(AppConstants.CacheMenuItems);
                if (cachedItems != null && cachedItems.Count > 0)
                {
                    return cachedItems;
                }
            }

            var response = await GetAsync<ApiResponse<List<MenuItem>>>(ApiEndpoints.GetMenuItems);

            if (response.Success && response.Data != null)
            {
                _cacheService.Set(AppConstants.CacheMenuItems, response.Data,
                    TimeSpan.FromMinutes(AppConstants.CacheDurationMedium));
                return response.Data;
            }

            return new List<MenuItem>();
        }

        public async Task<List<MenuItem>> GetMenuItemsByCategoryAsync(string categoryId, bool forceRefresh = false)
        {
            var cacheKey = $"{AppConstants.CacheMenuItems}_{categoryId}";

            if (!forceRefresh)
            {
                var cachedItems = _cacheService.Get<List<MenuItem>>(cacheKey);
                if (cachedItems != null && cachedItems.Count > 0)
                {
                    return cachedItems;
                }
            }

            var endpoint = string.Format(ApiEndpoints.GetMenuItemsByCategory, categoryId);
            var response = await GetAsync<ApiResponse<List<MenuItem>>>(endpoint);

            if (response.Success && response.Data != null)
            {
                _cacheService.Set(cacheKey, response.Data,
                    TimeSpan.FromMinutes(AppConstants.CacheDurationMedium));
                return response.Data;
            }

            return new List<MenuItem>();
        }

        public async Task<List<MenuItem>> GetFeaturedMenuItemsAsync(bool forceRefresh = false)
        {
            if (!forceRefresh)
            {
                var cachedItems = _cacheService.Get<List<MenuItem>>(AppConstants.CacheFeaturedItems);
                if (cachedItems != null && cachedItems.Count > 0)
                {
                    return cachedItems;
                }
            }

            var response = await GetAsync<ApiResponse<List<MenuItem>>>(ApiEndpoints.GetFeaturedMenuItems);

            if (response.Success && response.Data != null)
            {
                _cacheService.Set(AppConstants.CacheFeaturedItems, response.Data,
                    TimeSpan.FromMinutes(AppConstants.CacheDurationMedium));
                return response.Data;
            }

            return new List<MenuItem>();
        }

        public async Task<MenuItemDetail> GetMenuItemDetailAsync(string menuItemId)
        {
            var endpoint = string.Format(ApiEndpoints.GetMenuItem, menuItemId);
            var response = await GetAsync<ApiResponse<MenuItemDetail>>(endpoint);

            if (response.Success && response.Data != null)
            {
                return response.Data;
            }

            return null;
        }

        public async Task<List<MenuItem>> SearchMenuItemsAsync(string query)
        {
            var endpoint = string.Format(ApiEndpoints.SearchMenuItems, Uri.EscapeDataString(query));
            var response = await GetAsync<ApiResponse<List<MenuItem>>>(endpoint);

            if (response.Success && response.Data != null)
            {
                return response.Data;
            }

            return new List<MenuItem>();
        }

        public async Task<MenuItemDetail> CreateMenuItemAsync(MenuItemDetail menuItem, Stream imageStream = null)
        {
            if (imageStream != null)
            {
                var fileName = Guid.NewGuid().ToString() + ".jpg";
                var formData = await ImageHelper.CreateImageContentAsync(imageStream, fileName);

                // Add other fields
                formData.Add(new StringContent(menuItem.Name), "Name");
                formData.Add(new StringContent(menuItem.Description ?? string.Empty), "Description");
                formData.Add(new StringContent(menuItem.Price.ToString()), "Price");
                formData.Add(new StringContent(menuItem.CategoryId), "CategoryId");
                formData.Add(new StringContent(menuItem.IsAvailable.ToString()), "IsAvailable");
                formData.Add(new StringContent(menuItem.IsVegetarian.ToString()), "IsVegetarian");
                formData.Add(new StringContent(menuItem.IsVegan.ToString()), "IsVegan");
                formData.Add(new StringContent(menuItem.IsGlutenFree.ToString()), "IsGlutenFree");
                formData.Add(new StringContent(menuItem.PreparationTimeMinutes.ToString()), "PreparationTimeMinutes");
                formData.Add(new StringContent(menuItem.Calories.ToString()), "Calories");
                formData.Add(new StringContent(menuItem.DiscountPercentage.ToString()), "DiscountPercentage");
                formData.Add(new StringContent(menuItem.IsFeatured.ToString()), "IsFeatured");
                formData.Add(new StringContent(menuItem.DisplayOrder.ToString()), "DisplayOrder");

                // Add ingredients and allergens
                for (int i = 0; i < menuItem.Ingredients.Count; i++)
                {
                    formData.Add(new StringContent(menuItem.Ingredients[i]), $"Ingredients[{i}]");
                }

                for (int i = 0; i < menuItem.Allergens.Count; i++)
                {
                    formData.Add(new StringContent(menuItem.Allergens[i]), $"Allergens[{i}]");
                }

                var response =
                    await PostFormDataAsync<ApiResponse<MenuItemDetail>>(ApiEndpoints.CreateMenuItem, formData);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuItems);
                    _cacheService.Remove(AppConstants.CacheFeaturedItems);
                    _cacheService.Remove($"{AppConstants.CacheMenuItems}_{menuItem.CategoryId}");
                    return response.Data;
                }
            }
            else
            {
                var response = await PostAsync<ApiResponse<MenuItemDetail>>(ApiEndpoints.CreateMenuItem, menuItem);

                if (response.Success && response.Data != null)
                {
                    // Clear cache
                    _cacheService.Remove(AppConstants.CacheMenuItems);
                    _cacheService.Remove(AppConstants.CacheFeaturedItems);
                    _cacheService.Remove($"{AppConstants.CacheMenuItems}_{menuItem.CategoryId}");
                    return response.Data;
                }
            }

            return null;
        }
        
        public async Task<MenuItemDetail> UpdateMenuItemAsync(MenuItemDetail menuItem, Stream imageStream = null)
            {
                var endpoint = string.Format(ApiEndpoints.UpdateMenuItem, menuItem.Id);
                
                if (imageStream != null)
                {
                    var fileName = Guid.NewGuid().ToString() + ".jpg";
                    var formData = await ImageHelper.CreateImageContentAsync(imageStream, fileName);
                    
                    // Add other fields
                    formData.Add(new StringContent(menuItem.Name), "Name");
                    formData.Add(new StringContent(menuItem.Description ?? string.Empty), "Description");
                    formData.Add(new StringContent(menuItem.Price.ToString()), "Price");
                    formData.Add(new StringContent(menuItem.CategoryId), "CategoryId");
                    formData.Add(new StringContent(menuItem.IsAvailable.ToString()), "IsAvailable");
                    formData.Add(new StringContent(menuItem.IsVegetarian.ToString()), "IsVegetarian");
                    formData.Add(new StringContent(menuItem.IsVegan.ToString()), "IsVegan");
                    formData.Add(new StringContent(menuItem.IsGlutenFree.ToString()), "IsGlutenFree");
                    formData.Add(new StringContent(menuItem.PreparationTimeMinutes.ToString()), "PreparationTimeMinutes");
                    formData.Add(new StringContent(menuItem.Calories.ToString()), "Calories");
                    formData.Add(new StringContent(menuItem.DiscountPercentage.ToString()), "DiscountPercentage");
                    formData.Add(new StringContent(menuItem.IsFeatured.ToString()), "IsFeatured");
                    formData.Add(new StringContent(menuItem.DisplayOrder.ToString()), "DisplayOrder");
                    
                    // Add ingredients and allergens
                    for (int i = 0; i < menuItem.Ingredients.Count; i++)
                    {
                        formData.Add(new StringContent(menuItem.Ingredients[i]), $"Ingredients[{i}]");
                    }
                    
                    for (int i = 0; i < menuItem.Allergens.Count; i++)
                    {
                        formData.Add(new StringContent(menuItem.Allergens[i]), $"Allergens[{i}]");
                    }
                    
                    var response = await PutFormDataAsync<ApiResponse<MenuItemDetail>>(endpoint, formData);
                    
                    if (response.Success && response.Data != null)
                    {
                        // Clear cache
                        _cacheService.Remove(AppConstants.CacheMenuItems);
                        _cacheService.Remove(AppConstants.CacheFeaturedItems);
                        _cacheService.Remove($"{AppConstants.CacheMenuItems}_{menuItem.CategoryId}");
                        return response.Data;
                    }
                }
                else
                {
                    var response = await PutAsync<ApiResponse<MenuItemDetail>>(endpoint, menuItem);
                    
                    if (response.Success && response.Data != null)
                    {
                        // Clear cache
                        _cacheService.Remove(AppConstants.CacheMenuItems);
                        _cacheService.Remove(AppConstants.CacheFeaturedItems);
                        _cacheService.Remove($"{AppConstants.CacheMenuItems}_{menuItem.CategoryId}");
                        return response.Data;
                    }
                }
                
                return null;
            }

            public async Task<bool> UpdateMenuItemAvailabilityAsync(string menuItemId, bool isAvailable)
            {
            var endpoint = string.Format(ApiEndpoints.UpdateMenuItemAvailability, menuItemId);
            var request = new { IsAvailable = isAvailable };

            var response = await PutAsync<ApiResponse<bool>>(endpoint, request);

            if (response.Success)
            {
                // Clear cache
                _cacheService.Remove(AppConstants.CacheMenuItems);
                _cacheService.Remove(AppConstants.CacheFeaturedItems);
                return true;
            }

            return false;
            }

            public async Task<bool> DeleteMenuItemAsync(string menuItemId)
            {
            var endpoint = string.Format(ApiEndpoints.DeleteMenuItem, menuItemId);
            var response = await DeleteAsync<ApiResponse<bool>>(endpoint);

            if (response.Success)
            {
                // Clear cache
                _cacheService.Remove(AppConstants.CacheMenuItems);
                _cacheService.Remove(AppConstants.CacheFeaturedItems);
                return true;
            }

            return false;
            }

    }
}