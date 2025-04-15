using System.Windows.Input;
using Shared.Models.Menu;

namespace Client.Controls
{
    public partial class MenuItemCard : ContentView
    {
        public static readonly BindableProperty MenuItemProperty =
            BindableProperty.Create(nameof(MenuItem), typeof(Shared.Models.Menu.MenuItem), typeof(MenuItemCard), null, propertyChanged: OnMenuItemChanged);

        public static readonly BindableProperty ViewCommandProperty =
            BindableProperty.Create(nameof(ViewCommand), typeof(ICommand), typeof(MenuItemCard));

        public Shared.Models.Menu.MenuItem MenuItem
        {
            get => (Shared.Models.Menu.MenuItem)GetValue(MenuItemProperty);
            set => SetValue(MenuItemProperty, value);
        }
        
        public ICommand ViewCommand
        {
            get => (ICommand)GetValue(ViewCommandProperty);
            set => SetValue(ViewCommandProperty, value);
        }

        public MenuItemCard()
        {
            InitializeComponent();
            ViewButton.Clicked += OnViewButtonClicked;
        }

        private void OnViewButtonClicked(object sender, EventArgs e)
        {
            if (ViewCommand?.CanExecute(MenuItem) == true)
            {
                ViewCommand.Execute(MenuItem);
            }
        }

        private static void OnMenuItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (MenuItemCard)bindable;
            
            if (newValue is Shared.Models.Menu.MenuItem menuItem)
            {
                // Set image
                control.ItemImage.Source = menuItem.ImageUrl;
                
                // Set name
                control.NameLabel.Text = menuItem.Name;
                
                // Set category
                control.CategoryLabel.Text = menuItem.CategoryName;
                
                // Handle discount
                if (menuItem.DiscountPercentage > 0)
                {
                    control.DiscountFrame.IsVisible = true;
                    control.DiscountLabel.Text = $"{menuItem.DiscountPercentage}% OFF";
                    
                    control.OriginalPriceLabel.IsVisible = true;
                    control.OriginalPriceLabel.Text = $"${menuItem.Price:F2}";
                    
                    decimal discountedPrice = menuItem.Price * (1 - (menuItem.DiscountPercentage / 100));
                    control.PriceLabel.Text = $"${discountedPrice:F2}";
                }
                else
                {
                    control.DiscountFrame.IsVisible = false;
                    control.OriginalPriceLabel.IsVisible = false;
                    control.PriceLabel.Text = $"${menuItem.Price:F2}";
                }
                
                // Handle availability
                control.Opacity = menuItem.IsAvailable ? 1.0 : 0.6;
            }
        }
    }
}