namespace Gym_API.DTOs
{
    public class RenewResponse
    {
        public int MemberId { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
