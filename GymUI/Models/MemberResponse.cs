
namespace GymUI.Models
{
    public class MemberResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public DateOnly JoinDate { get; set; }
        public bool IsActive { get; set; }
        public PlanSummary? Plan { get; set; }
        public List<TrainerSummary> Trainers { get; set; } = [];
        public DateOnly? ExpiryDate { get; set; }
    }
}
