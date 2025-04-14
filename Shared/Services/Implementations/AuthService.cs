using Shared.Constants;
using Shared.Models.Auth;
using Shared.Models.Common;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class AuthService : ServiceBase, IAuthService
    {
        public AuthService(HttpClient httpClient, ISettingsService settingsService) 
            : base(httpClient, settingsService)
        {
        }

        public async Task<AuthResponse> LoginAsync(string username, string password)
        {
            var request = new LoginRequest
            {
                Username = username,
                Password = password
            };

            var response = await PostAsync<ApiResponse<AuthResponse>>(ApiEndpoints.Login, request);

            if (response.Success && response.Data != null)
            {
                // Save auth info
                _settingsService.AuthToken = response.Data.Token;
                _settingsService.UserId = response.Data.Id;
                _settingsService.Username = response.Data.Username;
                _settingsService.UserEmail = response.Data.Email;
                _settingsService.UserType = response.Data.UserType.ToString();
                _settingsService.TokenExpiration = response.Data.Expiration;
                _settingsService.SaveSettings();
                
                // Reset authorization header with new token
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Data.Token);
                
                return response.Data;
            }
            
            throw new UnauthorizedAccessException(response.Message ?? Messages.LoginFailed);
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var response = await PostAsync<ApiResponse<AuthResponse>>(ApiEndpoints.Register, request);
            
            if (response.Success && response.Data != null)
            {
                // Save auth info
                _settingsService.AuthToken = response.Data.Token;
                _settingsService.UserId = response.Data.Id;
                _settingsService.Username = response.Data.Username;
                _settingsService.UserEmail = response.Data.Email;
                _settingsService.UserType = response.Data.UserType.ToString();
                _settingsService.TokenExpiration = response.Data.Expiration;
                _settingsService.SaveSettings();
                
                // Reset authorization header with new token
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", response.Data.Token);
                
                return response.Data;
            }
            
            throw new Exception(response.Message ?? Messages.RegisterFailed);
        }

        public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = newPassword
            };
            
            var response = await PostAsync<ApiResponse<bool>>(ApiEndpoints.ChangePassword, request);
            return response.Success;
        }

        public async Task<UserProfile> GetProfileAsync()
        {
            if (!HasValidSession())
                throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);
            
            var response = await GetAsync<ApiResponse<UserProfile>>(ApiEndpoints.GetProfile);
            return response.Data;
        }

        public async Task<bool> UpdateProfileAsync(UserProfile profile)
        {
            if (!HasValidSession())
                throw new UnauthorizedAccessException(Messages.UnauthorizedAccess);
            
            var updateDto = new UpdateUserDto
            {
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber
            };
            
            var response = await PutAsync<ApiResponse<UserProfile>>(
                string.Format(ApiEndpoints.UpdateUser, profile.Id), updateDto);
            
            if (response.Success && response.Data != null)
            {
                _settingsService.UserEmail = response.Data.Email;
                _settingsService.SaveSettings();
                return true;
            }
            
            return false;
        }

        public Task<bool> LogoutAsync()
        {
            _settingsService.AuthToken = null;
            _settingsService.UserId = null;
            _settingsService.Username = null;
            _settingsService.UserEmail = null;
            _settingsService.UserType = null;
            _settingsService.TokenExpiration = null;
            _settingsService.SaveSettings();
            
            // Reset authorization header
            _httpClient.DefaultRequestHeaders.Authorization = null;
            
            return Task.FromResult(true);
        }

        public bool IsTokenExpired()
        {
            var expiration = _settingsService.TokenExpiration;
            return !expiration.HasValue || expiration.Value <= DateTime.UtcNow;
        }

        public bool HasValidSession()
        {
            return !string.IsNullOrEmpty(_settingsService.AuthToken) && 
                   !string.IsNullOrEmpty(_settingsService.UserId) && 
                   !IsTokenExpired();
        }
    }
    
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
    
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}