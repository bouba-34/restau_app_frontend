using Client.ViewModels.Auth;
using Client.ViewModels.Base;

namespace Client.Views.Auth
{
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;
        
        public LoginPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Login;
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