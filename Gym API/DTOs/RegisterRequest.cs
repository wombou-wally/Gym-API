using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Gym_API.DTOs
{
    public class RegisterRequest
    {
        [MaxLength(256)][EmailAddress][Required]public required string Email { get; set; }
        [Required][MinLength(8)][MaxLength(128)] public required string Password { get; set; }
        [Required] public required string Role { get; set; }
    }
}
