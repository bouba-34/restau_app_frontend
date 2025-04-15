using Client.ViewModels.Auth;
using Client.ViewModels.Base;

namespace Client.Views.Auth
{
    public partial class ProfilePage : ContentPage
    {
        private readonly ProfileViewModel _viewModel;
        
        public ProfilePage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Profile;
            BindingContext = _viewModel;
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