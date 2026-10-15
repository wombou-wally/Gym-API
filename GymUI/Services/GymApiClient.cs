using GymUI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GymUI.Services
{
    public class GymApiClient
    {
        private const string ServerDown = "Cannot reach the server. Is the API running?";
        private readonly HttpClient _httpClient;

        public GymApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ---------- auth ----------
        public async Task<ApiResult<LoginResponse>> LoginAsync(string email, string password)
        {
            try
            {
                var body = new LoginRequest { Email = email, Password = password };
                using var response = await _httpClient.PostAsJsonAsync("api/auth/login", body);

                if (response.IsSuccessStatusCode)
                {
                    return new ApiResult<LoginResponse>
                    {
                        Success = true,
                        Data = await response.Content.ReadFromJsonAsync<LoginResponse>()
                    };
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return new ApiResult<LoginResponse> { Error = "Invalid email or password." };
                }

                var text = await response.Content.ReadAsStringAsync();
                return new ApiResult<LoginResponse> { Error = ReadError(text) };
            }
            catch (HttpRequestException)
            {
                return new ApiResult<LoginResponse> { Error = ServerDown };
            }
        }

        // ---------- reads ----------
        public Task<ApiResult<List<MemberResponse>>> GetMembersAsync(string token) =>
            GetAsync<List<MemberResponse>>("api/members", token);

        public Task<ApiResult<List<PlanResponse>>> GetPlansAsync(string token) =>
            GetAsync<List<PlanResponse>>("api/plans", token);

        public Task<ApiResult<List<TrainerResponse>>> GetTrainersAsync(string token) =>
            GetAsync<List<TrainerResponse>>("api/trainers", token);

        public Task<ApiResult<List<CheckInResponse>>> GetCheckInsAsync(string token) =>
            GetAsync<List<CheckInResponse>>("api/checkins", token);

        // ---------- writes ----------
        public Task<ApiResult> CreateMemberAsync(string token, CreateMemberRequest member) =>
            SendAsync(HttpMethod.Post, "api/members", token, member);

        public Task<ApiResult> CreatePlanAsync(string token, CreatePlanRequest plan) =>
            SendAsync(HttpMethod.Post, "api/plans", token, plan);

        public Task<ApiResult> CreateTrainerAsync(string token, CreateTrainerRequest trainer) =>
            SendAsync(HttpMethod.Post, "api/trainers", token, trainer);

        public Task<ApiResult> CheckInAsync(string token, int memberId) =>
            SendAsync(HttpMethod.Post, "api/checkins", token, new CreateCheckInRequest { MemberId = memberId });

        public Task<ApiResult> RenewAsync(string token, int memberId) =>
            SendAsync(HttpMethod.Post, $"api/members/{memberId}/renew", token);

        public Task<ApiResult> DeleteMemberAsync(string token, int memberId) =>
            SendAsync(HttpMethod.Delete, $"api/members/{memberId}", token);

        public Task<ApiResult> DeletePlanAsync(string token, int planId) =>
            SendAsync(HttpMethod.Delete, $"api/plans/{planId}", token);

        // ---------- helpers ----------
        private static HttpRequestMessage BuildRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        private async Task<ApiResult<T>> GetAsync<T>(string url, string token)
        {
            try
            {
                using var request = BuildRequest(HttpMethod.Get, url, token);
                using var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var failure = await FailureAsync(response);
                    return new ApiResult<T> { Error = failure.Error, SessionExpired = failure.SessionExpired };
                }

                return new ApiResult<T>
                {
                    Success = true,
                    Data = await response.Content.ReadFromJsonAsync<T>()
                };
            }
            catch (HttpRequestException)
            {
                return new ApiResult<T> { Error = ServerDown };
            }
        }

        private async Task<ApiResult> SendAsync(HttpMethod method, string url, string token, object? body = null)
        {
            try
            {
                using var request = BuildRequest(method, url, token);
                if (body != null)
                {
                    request.Content = JsonContent.Create(body);
                }

                using var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return new ApiResult { Success = true };
                }
                return await FailureAsync(response);
            }
            catch (HttpRequestException)
            {
                return new ApiResult { Error = ServerDown };
            }
        }

        private static async Task<ApiResult> FailureAsync(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ApiResult { Error = "Your session expired. Please sign in again.", SessionExpired = true };
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return new ApiResult { Error = "You do not have permission to do that." };
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new ApiResult { Error = "That item no longer exists." };
            }

            var text = await response.Content.ReadAsStringAsync();
            return new ApiResult { Error = ReadError(text) };
        }

        private static string ReadError(string body)
        {
            try
            {
                var problem = JsonSerializer.Deserialize<ValidationProblemResponse>(body, JsonSerializerOptions.Web);
                if (problem?.Errors != null && problem.Errors.Count > 0)
                {
                    return string.Join(" ", problem.Errors.SelectMany(e => e.Value));
                }
            }
            catch (JsonException)
            {
                // Plain-text 400 from a controller. Show it as is.
            }

            return string.IsNullOrWhiteSpace(body) ? "The request failed." : body;
        }
    }
}