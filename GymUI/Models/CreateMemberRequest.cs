namespace GymUI.Models
{
    public class CreateMemberRequest
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public DateOnly JoinDate { get; set; }
        public int? MembershipPlanId { get; set; }
    }
}