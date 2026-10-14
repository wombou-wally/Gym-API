namespace Gym_API.DTOs
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public required DateTime ExpiresAt { get; set; }
        public required string Role { get; set; }
    }
}
