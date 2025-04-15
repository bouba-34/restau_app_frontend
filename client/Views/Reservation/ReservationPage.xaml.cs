using Client.ViewModels.Base;
using Client.ViewModels.Reservation;

namespace Client.Views.Reservation
{
    public partial class ReservationPage : ContentPage
    {
        private readonly ReservationViewModel _viewModel;
        
        public ReservationPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.Reservation;
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