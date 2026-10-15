namespace GymUI.Models
{
    public class LoginResponse
    {
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public required string Role { get; set; }
    }
}