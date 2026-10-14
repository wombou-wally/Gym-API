using System.ComponentModel.DataAnnotations;

namespace Gym_API.DTOs
{
    public class LoginRequest
    {
        [EmailAddress][Required][MaxLength(256)]public required string Email { get; set; }
        [Required][MaxLength(128)] public required string Password { get; set; }
    }
}
