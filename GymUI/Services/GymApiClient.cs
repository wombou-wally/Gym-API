using GymUI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GymUI.Services
{
    public class GymApiClient
    {
        private readonly HttpClient _httpClient;

        public GymApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, string url, string token)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return request;
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<List<MemberResponse>?> GetMembersAsync(string token)
        {
            using var request = BuildRequest(HttpMethod.Get, "api/members", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<MemberResponse>>();
        }

        public async Task<List<PlanResponse>?> GetPlansAsync(string token)
        {
            using var request = BuildRequest(HttpMethod.Get, "api/plans", token);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<List<PlanResponse>>();
        }

        public async Task<ApiResult> CreateMemberAsync(string token, CreateMemberRequest member)
        {
            using var request = BuildRequest(HttpMethod.Post, "api/members", token);
            request.Content = JsonContent.Create(member);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return new ApiResult { Success = true };
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return new ApiResult { Error = "Your session expired. Please log in again." };
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return new ApiResult { Error = "You do not have permission to do that." };
            }

            var body = await response.Content.ReadAsStringAsync();
            return new ApiResult { Error = ReadError(body) };
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
                // The body was plain text, not JSON. Fall through and show it as is.
            }

            return string.IsNullOrWhiteSpace(body) ? "The request failed." : body;
        }
    }
}