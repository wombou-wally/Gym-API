namespace Gym_API.Models
{
    public class MembershipPlan
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public List<Member> Members { get; set; } = [];
    }
}