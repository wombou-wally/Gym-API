using System.Text.Json.Serialization;

namespace Gym_API.Models
{
    public class CheckIn
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime CheckInTime { get; set; }
        public Member? member { get; set; }

    }
}
