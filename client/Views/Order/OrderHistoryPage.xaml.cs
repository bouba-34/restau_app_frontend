using Client.ViewModels.Base;
using Client.ViewModels.Order;

namespace Client.Views.Order
{
    public partial class OrderHistoryPage : ContentPage
    {
        private readonly OrderHistoryViewModel _viewModel;
        
        public OrderHistoryPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.OrderHistory;
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