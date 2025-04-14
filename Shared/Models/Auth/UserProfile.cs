namespace Shared.Models.Auth
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}