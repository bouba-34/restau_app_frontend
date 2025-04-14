using Client.ViewModels.Base;
using Client.ViewModels.Menu;

namespace Client.Views.Menu
{
    public partial class MenuPage : ContentPage
    {
        private readonly MenuViewModel _viewModel;
        
        public MenuPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Menu;
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