using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;

namespace admin.ViewModels
{
    [QueryProperty(nameof(ReservationId), "ReservationId")]
    public partial class ReservationDetailViewModel : BaseViewModel
    {
        private readonly IReservationService _reservationService;
        private readonly INotificationService _notificationService;

        [ObservableProperty]
        private string reservationId;

        [ObservableProperty]
        private Reservation reservation;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private ReservationStatus nextStatus;

        [ObservableProperty]
        private bool canUpdateStatus;

        [ObservableProperty]
        private string nextStatusButtonText;

        [ObservableProperty]
        private bool canCancelReservation;

        public ReservationDetailViewModel(
            IReservationService reservationService,
            INotificationService notificationService)
        {
            Title = "Reservation Details";
            _reservationService = reservationService;
            _notificationService = notificationService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await LoadReservationAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await LoadReservationAsync();
            IsRefreshing = false;
        }

        partial void OnReservationIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value) && IsInitialized)
            {
                LoadReservationAsync().ConfigureAwait(false);
            }
        }

        private async Task LoadReservationAsync()
        {
            if (string.IsNullOrEmpty(ReservationId))
                return;

            try
            {
                IsLoading = true;
                ClearError();

                Reservation = await _reservationService.GetReservationByIdAsync(ReservationId);
                
                if (Reservation != null)
                {
                    // Set next status and button text
                    nextStatus = GetNextReservationStatus(Reservation.Status);
                    NextStatusButtonText = GetStatusButtonText(Reservation.Status);
                    CanUpdateStatus = Reservation.Status != ReservationStatus.Completed && 
                                   Reservation.Status != ReservationStatus.Cancelled &&
                                   Reservation.Status != ReservationStatus.NoShow;
                    CanCancelReservation = Reservation.Status != ReservationStatus.Completed && 
                                       Reservation.Status != ReservationStatus.Cancelled &&
                                       Reservation.Status != ReservationStatus.NoShow;
                    
                    Title = $"Reservation: {Reservation.CustomerName}";
                    Subtitle = $"{Reservation.ReservationDate:MM/dd/yyyy} at {Reservation.ReservationTime}";
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load reservation: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private ReservationStatus GetNextReservationStatus(ReservationStatus currentStatus)
        {
            return currentStatus switch
            {
                ReservationStatus.Pending => ReservationStatus.Confirmed,
                ReservationStatus.Confirmed => ReservationStatus.Completed,
                _ => currentStatus
            };
        }

        private string GetStatusButtonText(ReservationStatus currentStatus)
        {
            return currentStatus switch
            {
                ReservationStatus.Pending => "Confirm Reservation",
                ReservationStatus.Confirmed => "Mark as Completed",
                _ => "Update Status"
            };
        }

        [RelayCommand]
        private async Task UpdateStatusAsync()
        {
            if (Reservation == null)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _reservationService.UpdateReservationStatusAsync(ReservationId, NextStatus);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Reservation.CustomerId,
                        $"Reservation Status Update",
                        $"Your reservation for {Reservation.ReservationDate:MM/dd/yyyy} at {Reservation.ReservationTime} is now {NextStatus}.");

                    // Reload reservation to get latest status
                    await LoadReservationAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to update reservation status: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelReservationAsync()
        {
            if (Reservation == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Cancel Reservation",
                "Are you sure you want to cancel this reservation?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _reservationService.CancelReservationAsync(ReservationId);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Reservation.CustomerId,
                        "Reservation Cancelled",
                        $"Your reservation for {Reservation.ReservationDate:MM/dd/yyyy} at {Reservation.ReservationTime} has been cancelled.");

                    // Reload reservation to get latest status
                    await LoadReservationAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to cancel reservation: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task MarkNoShowAsync()
        {
            if (Reservation == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Mark as No-Show",
                "Are you sure you want to mark this reservation as a no-show?",
                "Yes", "No");

            if (!confirm)
                return;

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _reservationService.UpdateReservationStatusAsync(ReservationId, ReservationStatus.NoShow);
                if (success)
                {
                    // Send notification to customer
                    await _notificationService.SendNotificationAsync(
                        Reservation.CustomerId,
                        "Reservation Marked as No-Show",
                        $"Your reservation for {Reservation.ReservationDate:MM/dd/yyyy} at {Reservation.ReservationTime} was marked as a no-show.");

                    // Reload reservation to get latest status
                    await LoadReservationAsync();
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to mark reservation as no-show: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}