using Client.ViewModels.Auth;
using Client.ViewModels.Base;

namespace Client.Views.Auth
{
    public partial class RegisterPage : ContentPage
    {
        private readonly RegisterViewModel _viewModel;
        
        public RegisterPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Register;
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