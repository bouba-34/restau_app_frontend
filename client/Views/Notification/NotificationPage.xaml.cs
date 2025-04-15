using Client.ViewModels.Base;
using Client.ViewModels.Notification;

namespace Client.Views.Notification
{
    public partial class NotificationPage : ContentPage
    {
        private readonly NotificationViewModel _viewModel;
        
        public NotificationPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Notification;
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