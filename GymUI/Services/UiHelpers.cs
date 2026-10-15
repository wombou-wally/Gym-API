namespace GymUI.Services
{
    public static class UiHelpers
    {
        public static string Initials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "?";
            }

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            return parts.Length == 1
                ? parts[0][..1].ToUpperInvariant()
                : (parts[0][..1] + parts[^1][..1]).ToUpperInvariant();
        }

        // Status comes from the expiry date, so it never goes stale.
        public static (string Text, string Css) Status(DateOnly? expiry)
        {
            if (expiry is null)
            {
                return ("Inactive", "inactive");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            return expiry.Value < today ? ("Expired", "expired") : ("Active", "active");
        }
    }
}