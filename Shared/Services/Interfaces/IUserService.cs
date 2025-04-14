using Shared.Models.Auth;

namespace Shared.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserProfile>> GetAllUsersAsync();
        Task<UserProfile> GetUserByIdAsync(string userId);
        Task<UserProfile> GetUserByUsernameAsync(string username);
        Task<UserProfile> GetUserByEmailAsync(string email);
        Task<List<UserProfile>> GetUsersByTypeAsync(UserType userType);
        Task<string> CreateUserAsync(string username, string email, string phoneNumber, string password, UserType userType);
        Task<UserProfile> UpdateUserAsync(string userId, string email, string phoneNumber);
        Task<bool> UpdateUserStatusAsync(string userId, bool isActive);
        Task<bool> DeleteUserAsync(string userId);
    }
}