using Client.ViewModels.Base;
using Client.ViewModels.Settings;

namespace Client.Views.Settings
{
    public partial class SettingsPage : ContentPage
    {
        private readonly SettingsViewModel _viewModel;
        
        public SettingsPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Settings;
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