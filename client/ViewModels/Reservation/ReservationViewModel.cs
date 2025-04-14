using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Helpers;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Reservation
{
    public partial class ReservationViewModel : ViewModelBase
    {
        private readonly IReservationService _reservationService;
        private readonly ISettingsService _settingsService;
        
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Today;
        
        [ObservableProperty]
        private TimeSpan _selectedTime = new TimeSpan(19, 0, 0); // Default 7 PM
        
        [ObservableProperty]
        private int _partySize = 2;
        
        [ObservableProperty]
        private string _specialRequests;
        
        [ObservableProperty]
        private string _contactPhone;
        
        [ObservableProperty]
        private string _contactEmail;
        
        [ObservableProperty]
        private ObservableCollection<AvailableTable> _availableTables;
        
        [ObservableProperty]
        private AvailableTable _selectedTable;
        
        [ObservableProperty]
        private bool _isSearching;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _noTablesAvailable;
        
        [ObservableProperty]
        private DateTime _minimumDate = DateTime.Today;
        
        [ObservableProperty]
        private DateTime _maximumDate = DateTime.Today.AddMonths(3);
        
        [ObservableProperty]
        private ObservableCollection<string> _validationErrors;

        public ReservationViewModel(
            INavigationService navigationService,
            IDialogService dialogService,
            IReservationService reservationService,
            ISettingsService settingsService)
            : base(navigationService, dialogService)
        {
            _reservationService = reservationService;
            _settingsService = settingsService;
            
            Title = "Make Reservation";
            AvailableTables = new ObservableCollection<AvailableTable>();
            ValidationErrors = new ObservableCollection<string>();
            
            // Set user contact information
            ContactPhone = _settingsService.GetValue<string>("UserPhone");
            ContactEmail = _settingsService.UserEmail;
        }
        
        [RelayCommand]
        private async Task CheckAvailabilityAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                // Clear validation errors
                ValidationErrors.Clear();
                
                // Validate input
                var errors = ValidationHelper.ValidateReservationRequest(SelectedDate, SelectedTime, PartySize);
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ValidationErrors.Add(error);
                    }
                    return;
                }
                
                IsSearching = true;
                NoTablesAvailable = false;
                
                try
                {
                    var tables = await _reservationService.GetAvailableTablesAsync(
                        SelectedDate, SelectedTime, PartySize);
                    
                    AvailableTables.Clear();
                    foreach (var table in tables.OrderBy(t => t.Capacity))
                    {
                        AvailableTables.Add(table);
                    }
                    
                    if (AvailableTables.Count > 0)
                    {
                        SelectedTable = AvailableTables.First();
                    }
                    else
                    {
                        NoTablesAvailable = true;
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Error checking availability", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        [RelayCommand]
        private async Task MakeReservationAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                if (SelectedTable == null)
                {
                    await DialogService.DisplayAlertAsync("No Table Selected", "Please search for and select an available table first", "OK");
                    return;
                }
                
                // Clear validation errors
                ValidationErrors.Clear();
                
                // Create reservation request
                var request = new CreateReservationRequest
                {
                    ReservationDate = SelectedDate,
                    ReservationTime = SelectedTime,
                    PartySize = PartySize,
                    SpecialRequests = SpecialRequests,
                    ContactPhone = ContactPhone,
                    ContactEmail = ContactEmail
                };
                
                try
                {
                    var reservationId = await _reservationService.CreateReservationAsync(request);
                    
                    if (string.IsNullOrEmpty(reservationId))
                    {
                        await DialogService.DisplayAlertAsync("Reservation Failed", "Failed to create reservation. Please try again.", "OK");
                        return;
                    }
                    
                    // Show confirmation
                    await DialogService.DisplayToastAsync(Messages.ReservationCreated);
                    
                    // Navigate to reservation detail page
                    var parameters = new Dictionary<string, object>
                    {
                        { "ReservationId", reservationId }
                    };
                    
                    await NavigationService.NavigateToAsync("reservation/detail", parameters);
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to create reservation", ex);
                }
            }, "Creating your reservation...");
        }
        
        [RelayCommand]
        private async Task NavigateToReservationHistoryAsync()
        {
            await NavigationService.NavigateToAsync("reservation/history");
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            // Nothing to initialize for now
            await Task.CompletedTask;
        }
        
        partial void OnSelectedDateChanged(DateTime value)
        {
            // Reset available tables when date changes
            AvailableTables.Clear();
            IsSearching = false;
            NoTablesAvailable = false;
        }
        
        partial void OnSelectedTimeChanged(TimeSpan value)
        {
            // Reset available tables when time changes
            AvailableTables.Clear();
            IsSearching = false;
            NoTablesAvailable = false;
        }
        
        partial void OnPartySizeChanged(int value)
        {
            // Reset available tables when party size changes
            AvailableTables.Clear();
            IsSearching = false;
            NoTablesAvailable = false;
        }
    }
}