namespace GymUI.Models
{
    public class ApiResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public bool SessionExpired { get; set; }
    }

    public class ApiResult<T> : ApiResult
    {
        public T? Data { get; set; }
    }

    public class TrainerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Speciality { get; set; }
    }

    public class CreateTrainerRequest
    {
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Speciality { get; set; }
    }

    public class CreatePlanRequest
    {
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public int DurationDays { get; set; } = 30;
    }

    public class CheckInResponse
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime CheckInTime { get; set; }
    }

    public class CreateCheckInRequest
    {
        public int MemberId { get; set; }
    }
}