using Gym_API.Models;

namespace Gym_API.DTOs
{
    public class CheckInResponse
    {    
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}
