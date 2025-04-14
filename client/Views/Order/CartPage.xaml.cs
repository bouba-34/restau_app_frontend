using Client.ViewModels.Base;
using Client.ViewModels.Order;

namespace Client.Views.Order
{
    public partial class CartPage : ContentPage
    {
        private readonly CartViewModel _viewModel;
        
        public CartPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Cart;
            BindingContext = _viewModel;
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Initialize the view model
            await _viewModel.InitializeAsync();
            _viewModel.OnAppearing();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearing();
        }
    }
}