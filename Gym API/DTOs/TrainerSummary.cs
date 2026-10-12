namespace Gym_API.DTOs
{
    public class TrainerSummary
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Speciality { get; set; }
    }
}
