using Client.ViewModels.Base;
using Client.ViewModels.Reservation;

namespace Client.Views.Reservation
{
    public partial class ReservationDetailPage : ContentPage, IQueryAttributable
    {
        private readonly ReservationDetailViewModel _viewModel;
        
        public ReservationDetailPage(ViewModelLocator viewModelLocator)
        {
            InitializeComponent();
            
            // Get the view model from the locator
            _viewModel = viewModelLocator.ReservationDetail;
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