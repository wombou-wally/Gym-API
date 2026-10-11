using System.Text.Json.Serialization;

namespace Gym_API.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public string? Speciality { get; set; }
        [JsonIgnore]
        public List<Member> Members { get; set; } = [];
    }
}
