using GymUI.Models;

namespace GymUI.Services
{
    public class ToastService
    {
        private readonly AuthState _auth;

        public ToastService(AuthState auth)
        {
            _auth = auth;
        }

        public string? Text { get; private set; }
        public bool IsError { get; private set; }
        public int Id { get; private set; }

        public event Action? OnChange;

        public void Success(string text) => Show(text, false);
        public void Error(string text) => Show(text, true);

        public void Clear()
        {
            Text = null;
        }

        // Shows an error toast and returns true when the call failed.
        public bool Failed(ApiResult result)
        {
            if (result.Success)
            {
                return false;
            }

            Error(result.Error ?? "Something went wrong.");
            if (result.SessionExpired)
            {
                _auth.SignOut();
            }
            return true;
        }

        // Shows a success or error toast. Returns true when the call worked.
        public bool Report(ApiResult result, string successText)
        {
            if (Failed(result))
            {
                return false;
            }

            Success(successText);
            return true;
        }

        private void Show(string text, bool isError)
        {
            Text = text;
            IsError = isError;
            Id++;
            OnChange?.Invoke();
        }
    }
}