using System.ComponentModel.DataAnnotations;

namespace Gym_API.DTOs
{
    public class UpdateMemberRequest
    {
        [Required][MaxLength(256)] public required string Name { get; set; }
        [Required][EmailAddress] public required string Email { get; set; }
        [MaxLength(20)] public string? Phone { get; set; }
        public int? MembershipPlanId { get; set; }
    }
}
