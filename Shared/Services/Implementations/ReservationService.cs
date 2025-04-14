using Shared.Constants;
using Shared.Models.Common;
using Shared.Models.Reservation;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class ReservationService : ServiceBase, IReservationService
    {
        private readonly ICacheService _cacheService;
        private readonly ISettingsService _settingsService;
        
        public ReservationService(
            HttpClient httpClient,
            ISettingsService settingsService,
            ICacheService cacheService)
            : base(httpClient, settingsService)
        {
            _cacheService = cacheService;
            _settingsService = settingsService;
        }

        public async Task<Reservation> GetReservationByIdAsync(string reservationId)
        {
            var endpoint = string.Format(ApiEndpoints.GetReservation, reservationId);
            var response = await GetAsync<ApiResponse<Reservation>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return null;
        }

        public async Task<List<Reservation>> GetReservationsByCustomerIdAsync(string customerId, bool forceRefresh = false)
        {
            var cacheKey = string.Format(AppConstants.CacheUserReservations, customerId);
            
            if (!forceRefresh)
            {
                var cachedReservations = _cacheService.Get<List<Reservation>>(cacheKey);
                if (cachedReservations != null && cachedReservations.Count > 0)
                {
                    return cachedReservations;
                }
            }
            
            var endpoint = string.Format(ApiEndpoints.GetReservationsByCustomer, customerId);
            var response = await GetAsync<ApiResponse<List<Reservation>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                _cacheService.Set(cacheKey, response.Data, 
                    TimeSpan.FromMinutes(AppConstants.CacheDurationShort));
                return response.Data;
            }
            
            return new List<Reservation>();
        }

        public async Task<List<Reservation>> GetReservationsByDateAsync(DateTime date)
        {
            var endpoint = string.Format(ApiEndpoints.GetReservationsByDate, date.ToString("yyyy-MM-dd"));
            var response = await GetAsync<ApiResponse<List<Reservation>>>(endpoint);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<Reservation>();
        }

        public async Task<List<Reservation>> GetUpcomingReservationsAsync()
        {
            var response = await GetAsync<ApiResponse<List<Reservation>>>(ApiEndpoints.GetUpcomingReservations);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<Reservation>();
        }

        public async Task<string> CreateReservationAsync(CreateReservationRequest request)
        {
            var response = await PostAsync<ApiResponse<Reservation>>(ApiEndpoints.CreateReservation, request);
            
            if (response.Success && response.Data != null)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserReservations, userId));
                }
                
                return response.Data.Id;
            }
            
            return null;
        }

        public async Task<Reservation> UpdateReservationAsync(string reservationId, CreateReservationRequest request)
        {
            var endpoint = string.Format(ApiEndpoints.UpdateReservation, reservationId);
            var response = await PutAsync<ApiResponse<Reservation>>(endpoint, request);
            
            if (response.Success && response.Data != null)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserReservations, userId));
                }
                
                return response.Data;
            }
            
            return null;
        }

        public async Task<bool> UpdateReservationStatusAsync(string reservationId, ReservationStatus status)
        {
            var endpoint = string.Format(ApiEndpoints.UpdateReservationStatus, reservationId);
            var request = new { Status = status };
            
            var response = await PutAsync<ApiResponse<bool>>(endpoint, request);
            
            if (response.Success)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserReservations, userId));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<bool> CancelReservationAsync(string reservationId)
        {
            var endpoint = string.Format(ApiEndpoints.CancelReservation, reservationId);
            var response = await PostAsync<ApiResponse<bool>>(endpoint, new { });
            
            if (response.Success)
            {
                // Clear cache
                var userId = _settingsService.UserId;
                if (!string.IsNullOrEmpty(userId))
                {
                    _cacheService.Remove(string.Format(AppConstants.CacheUserReservations, userId));
                }
                
                return true;
            }
            
            return false;
        }

        public async Task<List<AvailableTable>> GetAvailableTablesAsync(DateTime date, TimeSpan time, int partySize)
        {
            var request = new CheckAvailabilityRequest
            {
                Date = date,
                Time = time,
                PartySize = partySize
            };
            
            var response = await PostAsync<ApiResponse<List<AvailableTable>>>(ApiEndpoints.GetAvailableTables, request);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return new List<AvailableTable>();
        }

        public async Task<bool> IsTableAvailableAsync(string tableNumber, DateTime date, TimeSpan time)
        {
            var request = new
            {
                TableNumber = tableNumber,
                Date = date,
                Time = time
            };
            
            var response = await PostAsync<ApiResponse<bool>>(ApiEndpoints.CheckTableAvailability, request);
            
            if (response.Success)
            {
                return response.Data;
            }
            
            return false;
        }

        public async Task<Reservation> GetCurrentReservationAsync()
        {
            var userId = _settingsService.UserId;
            if (string.IsNullOrEmpty(userId))
                return null;
            
            var reservations = await GetReservationsByCustomerIdAsync(userId, true);
            
            // Find upcoming reservation (today or future)
            var currentDate = DateTime.Now.Date;
            var currentTime = DateTime.Now.TimeOfDay;
            
            var upcomingReservation = reservations
                .Where(r => r.Status != ReservationStatus.Cancelled && r.Status != ReservationStatus.NoShow)
                .Where(r => r.ReservationDate > currentDate || 
                           (r.ReservationDate == currentDate && r.ReservationTime > currentTime))
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.ReservationTime)
                .FirstOrDefault();
            
            return upcomingReservation;
        }

        public async Task<ReservationStatus> GetReservationStatusAsync(string reservationId)
        {
            var reservation = await GetReservationByIdAsync(reservationId);
            return reservation?.Status ?? ReservationStatus.Cancelled;
        }

        public async Task<bool> HasActiveReservationAsync()
        {
            var reservation = await GetCurrentReservationAsync();
            return reservation != null;
        }
    }
}