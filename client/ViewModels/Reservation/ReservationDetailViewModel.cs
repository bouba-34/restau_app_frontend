using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Constants;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;

namespace Client.ViewModels.Reservation
{
    public partial class ReservationDetailViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly IReservationService _reservationService;
        private readonly ISignalRService _signalRService;
        private string _reservationId;
        
        [ObservableProperty]
        private Shared.Models.Reservation.Reservation _reservation;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _canCancel;
        
        [ObservableProperty]
        private bool _canModify;
        
        [ObservableProperty]
        private bool _isEditMode;
        
        [ObservableProperty]
        private DateTime _selectedDate;
        
        [ObservableProperty]
        private TimeSpan _selectedTime;
        
        [ObservableProperty]
        private int _partySize;
        
        [ObservableProperty]
        private string _specialRequests;
        
        [ObservableProperty]
        private string _contactPhone;
        
        [ObservableProperty]
        private string _contactEmail;
        
        [ObservableProperty]
        private ObservableCollection<string> _validationErrors;
        
        [ObservableProperty]
        private DateTime _minimumDate = DateTime.Today;
        
        [ObservableProperty]
        private DateTime _maximumDate = DateTime.Today.AddMonths(3);
        
        public ReservationDetailViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IReservationService reservationService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _reservationService = reservationService;
            _signalRService = signalRService;
            
            Title = "Reservation Details";
            ValidationErrors = new ObservableCollection<string>();
            
            // Subscribe to SignalR events
            _signalRService.ReservationStatusChanged += OnReservationStatusChanged;
        }
        
        ~ReservationDetailViewModel()
        {
            // Unsubscribe from SignalR events
            _signalRService.ReservationStatusChanged -= OnReservationStatusChanged;
        }
        
        private void OnReservationStatusChanged(object sender, (string ReservationId, ReservationStatus Status) e)
        {
            if (e.ReservationId == _reservationId)
            {
                // Update reservation status
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await LoadReservationAsync();
                });
            }
        }
        
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("ReservationId", out var reservationId))
            {
                _reservationId = reservationId.ToString();
                LoadReservationAsync().ConfigureAwait(false);
            }
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadReservationAsync();
        }
        
        private async Task LoadReservationAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                IsRefreshing = true;
                
                try
                {
                    // Load reservation details
                    Reservation = await _reservationService.GetReservationByIdAsync(_reservationId);
                    
                    if (Reservation != null)
                    {
                        // Update title
                        Title = $"Reservation for {Reservation.ReservationDate.ToShortDateString()} at {Reservation.ReservationTime.ToString(@"hh\:mm tt")}";
                        
                        // Initialize edit fields
                        SelectedDate = Reservation.ReservationDate;
                        SelectedTime = Reservation.ReservationTime;
                        PartySize = Reservation.PartySize;
                        SpecialRequests = Reservation.SpecialRequests;
                        ContactPhone = Reservation.ContactPhone;
                        ContactEmail = Reservation.ContactEmail;
                        
                        // Check if can cancel
                        CanCancel = Reservation.Status == ReservationStatus.Pending || 
                                   Reservation.Status == ReservationStatus.Confirmed;
                        
                        // Check if can modify
                        CanModify = Reservation.Status == ReservationStatus.Pending;
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to load reservation details", ex);
                }
                finally
                {
                    IsRefreshing = false;
                }
            });
        }
        
        [RelayCommand]
        private void ToggleEditMode()
        {
            IsEditMode = !IsEditMode;
            
            if (!IsEditMode)
            {
                // Reset validation errors
                ValidationErrors.Clear();
                
                // Reset fields to original values
                if (Reservation != null)
                {
                    SelectedDate = Reservation.ReservationDate;
                    SelectedTime = Reservation.ReservationTime;
                    PartySize = Reservation.PartySize;
                    SpecialRequests = Reservation.SpecialRequests;
                    ContactPhone = Reservation.ContactPhone;
                    ContactEmail = Reservation.ContactEmail;
                }
            }
        }
        
        [RelayCommand]
        private async Task SaveChangesAsync()
        {
            await ExecuteBusyAction(async () =>
            {
                // Clear validation errors
                ValidationErrors.Clear();
                
                // Validate inputs
                var errors = Shared.Helpers.ValidationHelper.ValidateReservationRequest(
                    SelectedDate, SelectedTime, PartySize);
                
                if (errors.Count > 0)
                {
                    foreach (var error in errors)
                    {
                        ValidationErrors.Add(error);
                    }
                    return;
                }
                
                // Create update request
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
                    // Update reservation
                    var updatedReservation = await _reservationService.UpdateReservationAsync(_reservationId, request);
                    
                    if (updatedReservation != null)
                    {
                        // Update local data
                        Reservation = updatedReservation;
                        
                        // Exit edit mode
                        IsEditMode = false;
                        
                        // Show success message
                        await DialogService.DisplayToastAsync("Reservation updated successfully");
                    }
                    else
                    {
                        ValidationErrors.Add("Failed to update reservation");
                    }
                }
                catch (Exception ex)
                {
                    ValidationErrors.Add(ex.Message);
                }
            }, "Updating reservation...");
        }
        
        [RelayCommand]
        private async Task CancelReservationAsync()
        {
            if (Reservation == null)
                return;
                
            bool confirm = await DialogService.DisplayAlertAsync(
                "Cancel Reservation", 
                Messages.ConfirmCancelReservation, 
                "Yes, Cancel", 
                "No");
                
            if (!confirm)
                return;
                
            await ExecuteBusyAction(async () =>
            {
                try
                {
                    bool success = await _reservationService.CancelReservationAsync(_reservationId);
                    
                    if (success)
                    {
                        // Refresh reservation to show cancelled status
                        await LoadReservationAsync();
                        
                        // Show confirmation
                        await DialogService.DisplayToastAsync("Reservation cancelled successfully");
                        
                        // Navigate back
                        await NavigationService.GoBackAsync();
                    }
                    else
                    {
                        await DialogService.DisplayAlertAsync("Error", "Failed to cancel reservation", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync("Failed to cancel reservation", ex);
                }
            }, "Cancelling reservation...");
        }
        
        [RelayCommand]
        private void IncrementPartySize()
        {
            if (PartySize < 20)
            {
                PartySize++;
            }
        }
        
        [RelayCommand]
        private void DecrementPartySize()
        {
            if (PartySize > 1)
            {
                PartySize--;
            }
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data
            if (!string.IsNullOrEmpty(_reservationId))
            {
                LoadReservationAsync().ConfigureAwait(false);
            }
        }
    }
}