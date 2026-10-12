namespace Gym_API.DTOs
{
    public class PlansResponse
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
    }
}