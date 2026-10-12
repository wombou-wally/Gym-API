using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace Gym_API.DTOs
{
    public class MemberResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public DateOnly JoinDate { get; set; }
        public bool IsActive { get; set; }
        public PlanSummary? Plan { get; set; }
        public List<TrainerSummary> Trainers { get; set; } = [];
        public DateOnly? ExpiryDate { get; set; }
    }
}
