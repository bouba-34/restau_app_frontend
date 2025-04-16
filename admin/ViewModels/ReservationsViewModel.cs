using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using System.Collections.ObjectModel;

namespace admin.ViewModels
{
    public partial class ReservationsViewModel : BaseViewModel
    {
        private readonly IReservationService _reservationService;

        [ObservableProperty]
        private ObservableCollection<Reservation> reservations = new();

        [ObservableProperty]
        private ReservationStatus selectedStatus = ReservationStatus.Pending;

        [ObservableProperty]
        private bool isFilterByDate;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;

        [ObservableProperty]
        private string searchText;

        [ObservableProperty]
        private ObservableCollection<ReservationStatus> reservationStatusOptions = new(Enum.GetValues(typeof(ReservationStatus)).Cast<ReservationStatus>());

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private int totalReservations;

        public ReservationsViewModel(IReservationService reservationService)
        {
            Title = "Reservations";
            _reservationService = reservationService;

            RefreshCommand = new AsyncRelayCommand(RefreshAsync);
        }

        public override async Task InitializeAsync()
        {
            await RefreshAsync();
            IsInitialized = true;
        }

        public override async Task RefreshAsync()
        {
            await LoadReservationsAsync();
            IsRefreshing = false;
        }

        private async Task LoadReservationsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                List<Reservation> reservationsList;

                if (IsFilterByDate)
                {
                    // Get reservations by date
                    reservationsList = await _reservationService.GetReservationsByDateAsync(SelectedDate);
                }
                else
                {
                    // Get all upcoming reservations
                    reservationsList = await _reservationService.GetUpcomingReservationsAsync();
                    
                    // Filter by status if needed
                    if (selectedStatus != ReservationStatus.Pending)
                    {
                        reservationsList = reservationsList.Where(r => r.Status == SelectedStatus).ToList();
                    }
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    reservationsList = reservationsList.Where(r => 
                        r.Id.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        r.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                Reservations.Clear();
                foreach (var reservation in reservationsList)
                {
                    Reservations.Add(reservation);
                }

                TotalReservations = Reservations.Count;
            }
            catch (Exception ex)
            {
                ShowError("Failed to load reservations: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task ApplyFiltersAsync()
        {
            await LoadReservationsAsync();
        }

        [RelayCommand]
        private async Task ViewReservationDetailsAsync(string reservationId)
        {
            if (string.IsNullOrEmpty(reservationId))
                return;

            var parameters = new Dictionary<string, object>
            {
                { "ReservationId", reservationId }
            };

            await Shell.Current.GoToAsync($"ReservationDetailPage", parameters);
        }

        [RelayCommand]
        private async Task UpdateReservationStatusAsync(string reservationId)
        {
            if (string.IsNullOrEmpty(reservationId))
                return;

            var reservation = Reservations.FirstOrDefault(r => r.Id == reservationId);
            if (reservation == null)
                return;

            // Get next status based on current status
            var nextStatus = GetNextReservationStatus(reservation.Status);

            try
            {
                IsLoading = true;
                ClearError();

                var success = await _reservationService.UpdateReservationStatusAsync(reservationId, nextStatus);
                if (success)
                {
                    // Update the reservation in the collection
                    reservation.Status = nextStatus;
                    
                    // Refresh the collection to update UI
                    int index = Reservations.IndexOf(reservation);
                    if (index >= 0)
                    {
                        Reservations.RemoveAt(index);
                        Reservations.Insert(index, reservation);
                    }
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

        private ReservationStatus GetNextReservationStatus(ReservationStatus currentStatus)
        {
            return currentStatus switch
            {
                ReservationStatus.Pending => ReservationStatus.Confirmed,
                ReservationStatus.Confirmed => ReservationStatus.Completed,
                _ => currentStatus
            };
        }

        [RelayCommand]
        private void ToggleFilterMode()
        {
            IsFilterByDate = !IsFilterByDate;
        }

        partial void OnSelectedStatusChanged(ReservationStatus value)
        {
            if (IsInitialized && !IsFilterByDate)
            {
                LoadReservationsAsync().ConfigureAwait(false);
            }
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            if (IsInitialized && IsFilterByDate)
            {
                LoadReservationsAsync().ConfigureAwait(false);
            }
        }

        partial void OnSearchTextChanged(string value)
        {
            if (IsInitialized && !string.IsNullOrEmpty(value) && value.Length > 2)
            {
                LoadReservationsAsync().ConfigureAwait(false);
            }
            else if (IsInitialized && string.IsNullOrEmpty(value))
            {
                LoadReservationsAsync().ConfigureAwait(false);
            }
        }
    }
}