using Client.ViewModels.Base;
using Client.ViewModels.Menu;

namespace Client.Views.Menu
{
    public partial class MenuItemPage : ContentPage, IQueryAttributable
    {
        private readonly MenuItemViewModel _viewModel;
        
        public MenuItemPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.MenuItem;
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