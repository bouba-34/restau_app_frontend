using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Menu;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    [QueryProperty(nameof(MenuItemId), "MenuItemId")]
    [QueryProperty(nameof(IsNew), "IsNew")]
    public partial class MenuItemDetailViewModel : BaseViewModel
    {
        private readonly IMenuService _menuService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string menuItemId;

        [ObservableProperty]
        private bool isNew;

        [ObservableProperty]
        private MenuItemDetail menuItem;

        [ObservableProperty]
        private ObservableCollection<MenuCategory> categories = new();

        [ObservableProperty]
        private MenuCategory selectedCategory;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private bool isAvailable = true;

        [ObservableProperty]
        private bool isVegetarian;

        [ObservableProperty]
        private bool isVegan;

        [ObservableProperty]
        private bool isGlutenFree;

        [ObservableProperty]
        private bool isFeatured;

        [ObservableProperty]
        private int calories;

        [ObservableProperty]
        private decimal discountPercentage;

        [ObservableProperty]
        private string ingredients;

        [ObservableProperty]
        private string allergens;

        [ObservableProperty]
        private int preparationTimeMinutes = 15;

        [ObservableProperty]
        private string imageUrl;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isLoadingCategories;

        [ObservableProperty]
        private bool isImageUploading;

        [ObservableProperty]
        private string validationErrors;

        [ObservableProperty]
        private bool hasValidationErrors;

        private FileResult selectedImageFile;

        public MenuItemDetailViewModel(
            IMenuService menuService,
            INotificationService notificationService)
        {
            _menuService = menuService;
            _notificationService = notificationService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await LoadCategoriesAsync();
            
            if (IsNew)
            {
                Title = "Add Menu Item";
                InitializeNewMenuItem();
            }
            else
            {
                Title = "Edit Menu Item";
                await LoadMenuItemAsync();
            }
            
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            if (!IsNew)
            {
                await LoadMenuItemAsync();
            }
            
            await LoadCategoriesAsync();
            IsRefreshing = false;
        }

        private void InitializeNewMenuItem()
        {
            MenuItem = new MenuItemDetail
            {
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                Ingredients = new List<string>(),
                Allergens = new List<string>()
            };
            
            UpdateFormFields();
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsLoadingCategories = true;
                ClearError();

                var categoryList = await _menuService.GetCategoriesAsync(true);
                Categories.Clear();
                
                foreach (var category in categoryList)
                {
                    Categories.Add(category);
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

        private async Task LoadMenuItemAsync()
        {
            if (string.IsNullOrEmpty(MenuItemId))
                return;

            try
            {
                IsLoading = true;
                ClearError();

                MenuItem = await _menuService.GetMenuItemDetailAsync(MenuItemId);
                
                if (MenuItem != null)
                {
                    UpdateFormFields();
                    Subtitle = MenuItem.Name;
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load menu item: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void UpdateFormFields()
        {
            if (MenuItem == null)
                return;
                
            Name = MenuItem.Name;
            Description = MenuItem.Description;
            Price = MenuItem.Price;
            IsAvailable = MenuItem.IsAvailable;
            IsVegetarian = MenuItem.IsVegetarian;
            IsVegan = MenuItem.IsVegan;
            IsGlutenFree = MenuItem.IsGlutenFree;
            IsFeatured = MenuItem.IsFeatured;
            Calories = MenuItem.Calories;
            DiscountPercentage = MenuItem.DiscountPercentage;
            Ingredients = MenuItem.Ingredients != null ? string.Join(", ", MenuItem.Ingredients) : string.Empty;
            Allergens = MenuItem.Allergens != null ? string.Join(", ", MenuItem.Allergens) : string.Empty;
            PreparationTimeMinutes = MenuItem.PreparationTimeMinutes;
            ImageUrl = MenuItem.ImageUrl;
            
            // Set selected category
            if (!string.IsNullOrEmpty(MenuItem.CategoryId))
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == MenuItem.CategoryId);
            }
        }

        private void UpdateMenuItemFromForm()
        {
            if (MenuItem == null)
            {
                MenuItem = new MenuItemDetail();
            }
                
            MenuItem.Name = Name?.Trim();
            MenuItem.Description = Description?.Trim();
            MenuItem.Price = Price;
            MenuItem.IsAvailable = IsAvailable;
            MenuItem.IsVegetarian = IsVegetarian;
            MenuItem.IsVegan = IsVegan;
            MenuItem.IsGlutenFree = IsGlutenFree;
            MenuItem.IsFeatured = IsFeatured;
            MenuItem.Calories = Calories;
            MenuItem.DiscountPercentage = DiscountPercentage;
            MenuItem.Ingredients = string.IsNullOrEmpty(Ingredients) 
                ? new List<string>() 
                : Ingredients.Split(',').Select(i => i.Trim()).ToList();
            MenuItem.Allergens = string.IsNullOrEmpty(Allergens) 
                ? new List<string>() 
                : Allergens.Split(',').Select(a => a.Trim()).ToList();
            MenuItem.PreparationTimeMinutes = PreparationTimeMinutes;
            MenuItem.CategoryId = SelectedCategory?.Id;
            MenuItem.ImageUrl = ImageUrl;
        }

        private bool ValidateForm()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");
                
            if (Price <= 0)
                errors.Add("Price must be greater than zero");
                
            if (SelectedCategory == null)
                errors.Add("Category is required");
                
            if (PreparationTimeMinutes <= 0)
                errors.Add("Preparation time must be greater than zero");
                
            if (DiscountPercentage < 0 || DiscountPercentage > 100)
                errors.Add("Discount percentage must be between 0 and 100");
                
            ValidationErrors = string.Join(Environment.NewLine, errors);
            HasValidationErrors = errors.Count > 0;
            
            return !HasValidationErrors;
        }

        [RelayCommand]
        private async Task SelectImageAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select Image"
                });

                if (result != null)
                {
                    selectedImageFile = result;
                    
                    // Just update the UI to show the selected file name
                    // The actual upload happens when saving
                    ImageUrl = result.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowError("Error selecting image: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task SaveMenuItemAsync()
        {
            if (!ValidateForm())
                return;

            try
            {
                IsLoading = true;
                ClearError();
                
                // Update menu item from form
                UpdateMenuItemFromForm();
                
                // Prepare image stream if image was selected
                Stream imageStream = null;
                if (selectedImageFile != null)
                {
                    IsImageUploading = true;
                    imageStream = await selectedImageFile.OpenReadAsync();
                }
                
                try
                {
                    if (IsNew)
                    {
                        // Create new menu item
                        var result = await _menuService.CreateMenuItemAsync(MenuItem, imageStream);
                        
                        if (result != null)
                        {
                            await Shell.Current.DisplayAlert("Success", "Menu item created successfully", "OK");
                            await Shell.Current.GoToAsync("..");
                        }
                    }
                    else
                    {
                        // Update existing menu item
                        var result = await _menuService.UpdateMenuItemAsync(MenuItem, imageStream);
                        
                        if (result != null)
                        {
                            await Shell.Current.DisplayAlert("Success", "Menu item updated successfully", "OK");
                            await Shell.Current.GoToAsync("..");
                        }
                    }
                }
                finally
                {
                    // Clean up the image stream
                    if (imageStream != null)
                    {
                        await imageStream.DisposeAsync();
                        IsImageUploading = false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to save menu item: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task DeleteMenuItemAsync()
        {
            if (IsNew)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Menu Item",
                $"Are you sure you want to delete '{MenuItem.Name}'?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _menuService.DeleteMenuItemAsync(MenuItemId);
                
                if (success)
                {
                    await Shell.Current.DisplayAlert("Success", "Menu item deleted successfully", "OK");
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to delete menu item: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}