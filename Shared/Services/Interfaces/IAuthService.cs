using Shared.Models.Auth;

namespace Shared.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(string username, string password);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<UserProfile> GetProfileAsync();
        Task<bool> UpdateProfileAsync(UserProfile profile);
        Task<bool> LogoutAsync();
        bool IsTokenExpired();
        bool HasValidSession();
    }
}