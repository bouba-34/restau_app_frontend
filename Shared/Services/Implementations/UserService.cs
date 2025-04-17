using Shared.Models.Auth;
using Shared.Services.Helpers;
using Shared.Services.Interfaces;

namespace Shared.Services.Implementations
{
    public class UserService : ServiceBase, IUserService
    {
        public UserService(HttpClient httpClient, ISettingsService settingsService)
            : base(httpClient, settingsService)
        {
        }

        public async Task<List<UserProfile>> GetAllUsersAsync()
        {
            var endpoint = Shared.Constants.ApiEndpoints.GetAllUsers;
            return await GetAsync<List<UserProfile>>(endpoint);
        }

        public async Task<UserProfile> GetUserByIdAsync(string userId)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.GetUser, userId);
            return await GetAsync<UserProfile>(endpoint);
        }

        public async Task<UserProfile> GetUserByUsernameAsync(string username)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.GetUserByUsername, username);
            return await GetAsync<UserProfile>(endpoint);
        }

        public async Task<UserProfile> GetUserByEmailAsync(string email)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.GetUserByEmail, email);
            return await GetAsync<UserProfile>(endpoint);
        }

        public async Task<List<UserProfile>> GetUsersByTypeAsync(UserType userType)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.GetUsersByType, userType.ToString());
            return await GetAsync<List<UserProfile>>(endpoint);
        }

        public async Task<string> CreateUserAsync(string username, string email, string phoneNumber, string password, UserType userType)
        {
            var endpoint = Shared.Constants.ApiEndpoints.CreateUser;
            var request = new CreateUserDto
            {
                Username = username,
                Email = email,
                PhoneNumber = phoneNumber,
                Password = password,
                UserType = userType
            };
            
            var response = await PostAsync<Models.Common.ApiResponse<UserProfile>>(endpoint, request);
            
            if (response.Success && response.Data != null)
            {
                return response.Data.Id;
            }
            
            return null;
        }

        public async Task<UserProfile> UpdateUserAsync(string userId, string email, string phoneNumber)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.UpdateUser, userId);
            var request = new UpdateUserDto
            {
                Email = email,
                PhoneNumber = phoneNumber
            };
            
            var response = await PutAsync<Models.Common.ApiResponse<UserProfile>>(endpoint, request);
            
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            
            return null;
        }

        public async Task<bool> UpdateUserStatusAsync(string userId, bool isActive)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.UpdateUserStatus, userId);
            var request = new UpdateUserStatusDto
            {
                IsActive = isActive
            };
            
            var response = await PutAsync<Models.Common.ApiResponse<bool>>(endpoint, request);
            
            return response.Success && response.Data;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var endpoint = string.Format(Shared.Constants.ApiEndpoints.DeleteUser, userId);
            var response = await DeleteAsync<Models.Common.ApiResponse<bool>>(endpoint);
            
            return response.Success && response.Data;
        }
    }

    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
    }

    /*public class UpdateUserDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }*/

    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
    }
}