using Client.ViewModels.Base;
using Client.ViewModels.Order;

namespace Client.Views.Order
{
    public partial class OrderDetailPage : ContentPage, IQueryAttributable
    {
        private readonly OrderDetailViewModel _viewModel;
        
        public OrderDetailPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.OrderDetail;
            BindingContext = _viewModel;
        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // Forward query attributes to the view model
            (_viewModel as IQueryAttributable)?.ApplyQueryAttributes(query);
        }
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
        
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            _viewModel.OnDisappearing();
        }
    }
}