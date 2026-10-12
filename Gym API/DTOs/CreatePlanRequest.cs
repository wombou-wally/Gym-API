using System.ComponentModel.DataAnnotations;

namespace Gym_API.DTOs
{
    public class CreatePlanRequest
    {
        [Required][MaxLength(256)] public required string Name { get; set; }
        [Range(0,double.MaxValue)] public decimal Price { get; set; }
        [Range(1,int.MaxValue)]public int DurationDays { get; set; }
    }
}
