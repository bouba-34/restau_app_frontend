using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Client.ViewModels.Base;
using Shared.Models.Reservation;
using Shared.Services.Interfaces;
using Shared.Services.SignalR;
using System.Collections.ObjectModel;
using Client.Constants;

namespace Client.ViewModels.Reservation
{
    public partial class ReservationHistoryViewModel : ViewModelBase
    {
        private readonly IReservationService _reservationService;
        private readonly ISettingsService _settingsService;
        private readonly ISignalRService _signalRService;
        
        [ObservableProperty]
        private ObservableCollection<Shared.Models.Reservation.Reservation> _reservations;
        
        [ObservableProperty]
        private bool _isRefreshing;
        
        [ObservableProperty]
        private bool _isLoading;
        
        [ObservableProperty]
        private bool _hasUpcomingReservation;
        
        [ObservableProperty]
        private Shared.Models.Reservation.Reservation _upcomingReservation;
        
        [ObservableProperty]
        private bool _isEmpty;
        
        [ObservableProperty]
        private string _filterStatus = "All";
        
        public ObservableCollection<string> FilterOptions { get; } = new ObservableCollection<string>
        {
            "All", "Upcoming", "Completed", "Cancelled"
        };
        
        public ReservationHistoryViewModel(
            INavigationService navigationService, 
            IDialogService dialogService,
            IReservationService reservationService,
            ISettingsService settingsService,
            ISignalRService signalRService) 
            : base(navigationService, dialogService)
        {
            _reservationService = reservationService;
            _settingsService = settingsService;
            _signalRService = signalRService;
            
            Title = "My Reservations";
            Reservations = new ObservableCollection<Shared.Models.Reservation.Reservation>();
            
            // Subscribe to SignalR events
            _signalRService.ReservationStatusChanged += OnReservationStatusChanged;
            _signalRService.NewReservation += OnNewReservation;
        }
        
        ~ReservationHistoryViewModel()
        {
            // Unsubscribe from SignalR events
            _signalRService.ReservationStatusChanged -= OnReservationStatusChanged;
            _signalRService.NewReservation -= OnNewReservation;
        }
        
        private void OnReservationStatusChanged(object sender, (string ReservationId, ReservationStatus Status) e)
        {
            // Refresh reservation list
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadReservationsAsync(true);
            });
        }
        
        private void OnNewReservation(object sender, string reservationId)
        {
            // Refresh reservation list
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await LoadReservationsAsync(true);
            });
        }
        
        public override async Task InitializeAsync(object parameter = null)
        {
            await LoadReservationsAsync();
        }
        
        [RelayCommand]
        private async Task RefreshAsync()
        {
            await LoadReservationsAsync(true);
        }
        
        private async Task LoadReservationsAsync(bool forceRefresh = false)
        {
            IsLoading = true;
            IsRefreshing = true;
            
            try
            {
                var userId = _settingsService.UserId;
                
                if (string.IsNullOrEmpty(userId))
                {
                    return;
                }
                
                // Load reservations
                var allReservations = await _reservationService.GetReservationsByCustomerIdAsync(userId, forceRefresh);
                
                // Apply filter
                var filteredReservations = FilterReservations(allReservations);
                
                // Update collection
                Reservations.Clear();
                foreach (var reservation in filteredReservations)
                {
                    Reservations.Add(reservation);
                }
                
                // Check if there's an upcoming reservation
                var upcoming = allReservations.FirstOrDefault(r => 
                    (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed) &&
                    (r.ReservationDate > DateTime.Today || 
                     (r.ReservationDate == DateTime.Today && r.ReservationTime > DateTime.Now.TimeOfDay)));
                
                HasUpcomingReservation = upcoming != null;
                
                if (HasUpcomingReservation)
                {
                    UpcomingReservation = upcoming;
                }
                
                // Check if empty
                IsEmpty = Reservations.Count == 0;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load reservations", ex);
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }
        
        private List<Shared.Models.Reservation.Reservation> FilterReservations(List<Shared.Models.Reservation.Reservation> reservations)
        {
            return FilterStatus switch
            {
                "Upcoming" => reservations.Where(r => 
                    (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Confirmed) &&
                    (r.ReservationDate > DateTime.Today || 
                     (r.ReservationDate == DateTime.Today && r.ReservationTime > DateTime.Now.TimeOfDay)))
                    .OrderBy(r => r.ReservationDate)
                    .ThenBy(r => r.ReservationTime)
                    .ToList(),
                
                "Completed" => reservations.Where(r => r.Status == ReservationStatus.Completed)
                    .OrderByDescending(r => r.ReservationDate)
                    .ThenByDescending(r => r.ReservationTime)
                    .ToList(),
                
                "Cancelled" => reservations.Where(r => 
                    r.Status == ReservationStatus.Cancelled || r.Status == ReservationStatus.NoShow)
                    .OrderByDescending(r => r.ReservationDate)
                    .ThenByDescending(r => r.ReservationTime)
                    .ToList(),
                
                _ => reservations.OrderByDescending(r => r.ReservationDate)
                    .ThenByDescending(r => r.ReservationTime)
                    .ToList() // "All"
            };
        }
        
        partial void OnFilterStatusChanged(string value)
        {
            // Reload and filter reservations
            LoadReservationsAsync().ConfigureAwait(false);
        }
        
        [RelayCommand]
        private async Task ViewReservationDetailAsync(Shared.Models.Reservation.Reservation reservation)
        {
            if (reservation == null)
                return;
                
            var parameters = new Dictionary<string, object>
            {
                { "ReservationId", reservation.Id }
            };
            
            await NavigationService.NavigateToAsync(Routes.ReservationDetail, parameters);
        }
        
        [RelayCommand]
        private async Task MakeNewReservationAsync()
        {
            await NavigationService.NavigateToAsync(Routes.Reservation);
        }
        
        public override void OnAppearing()
        {
            base.OnAppearing();
            
            // Refresh data
            LoadReservationsAsync().ConfigureAwait(false);
        }
    }
}