namespace GymUI.Services
{
    public class AuthState
    {
        public string? Token { get; private set; }
        public string? Role { get; private set; }
        public bool IsLoggedIn => Token != null;
        public bool IsAdmin => Role == "Admin";

        public event Action? OnChange;

        public void SignIn(string token, string role)
        {
            Token = token;
            Role = role;
            OnChange?.Invoke();
        }

        public void SignOut()
        {
            Token = null;
            Role = null;
            OnChange?.Invoke();
        }
    }
}