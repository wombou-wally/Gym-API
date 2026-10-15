namespace GymUI.Services
{
    public class AuthState
    {
        public string? Token { get; private set; }
        public string? Role { get; private set; }
        public bool IsLoggedIn => Token != null;

        public void SignIn(string token, string role)
        {
            Token = token;
            Role = role;
        }

        public void SignOut()
        {
            Token = null;
            Role = null;
        }
    }
}