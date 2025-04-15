using Client.ViewModels.Base;
using Client.ViewModels.Reservation;

namespace Client.Views.Reservation
{
    public partial class ReservationHistoryPage : ContentPage
    {
        private readonly ReservationHistoryViewModel _viewModel;
        
        public ReservationHistoryPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.ReservationHistory;
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