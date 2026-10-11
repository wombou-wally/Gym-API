using System.Text.Json.Serialization;

namespace Gym_API.Models
{
    public class Member
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly JoinDate { get; set; }
        public bool IsActive { get; set; }
        public int? MembershipPlanId { get; set; }
        public MembershipPlan? MembershipPlan { get; set; }
        [JsonIgnore]
        public List<CheckIn> CheckIns { get; set; } = [];
        public List<Trainer> Trainers { get; set; } = [];
    }
}