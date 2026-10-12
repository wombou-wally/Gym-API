using System.ComponentModel.DataAnnotations;

namespace Gym_API.DTOs
{
    public class CreateMemberRequest
    {

        [Required][MaxLength(256)]public required string Name { get; set; }
        [Required][EmailAddress] public required string Email { get; set; }
        [MaxLength(20)]public string? Phone { get; set; }
        public DateOnly JoinDate { get; set; }
        public int? MembershipPlanId { get; set; }
        
    }
}
