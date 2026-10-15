using GymUI.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GymUI.Services
{
    public class GymApiClient
    {
        private readonly HttpClient _httpClient;
        public GymApiClient(HttpClient httpClient) 
            {
                _httpClient = httpClient;
            }
        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };

            var responce = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if(!responce.IsSuccessStatusCode)
            {
                return null;
            }

            return await responce.Content.ReadFromJsonAsync<LoginResponse>();
        }
         public async Task<List<MemberResponse>?> GetMembersAsync(string token)
         {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/members");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<MemberResponse>>();
         }
    }
}
