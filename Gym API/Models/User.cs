using System.ComponentModel.DataAnnotations;

namespace Gym_API.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(256)] public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        [MaxLength(20)] public required string Role { get; set; }
    }

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
    }
}