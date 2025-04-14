using Client.ViewModels.Base;
using Client.ViewModels.Menu;

namespace Client.Views.Menu
{
    public partial class MenuItemDetailPage : ContentPage, IQueryAttributable
    {
        private readonly MenuItemDetailViewModel _viewModel;
        
        public MenuItemDetailPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.MenuItemDetail;
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