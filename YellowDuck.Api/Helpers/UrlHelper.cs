namespace YellowDuck.Api.Helpers
{
    public static class UrlHelper
    {
        public static string ConfirmEmail(string to, string token, string lang = null)
        {
            var langParameter = (lang != null) ? $"&lang={lang}" : "";
            return $"confirmez-courriel?email={System.Net.WebUtility.UrlEncode(to)}&token={System.Net.WebUtility.UrlEncode(token)}{langParameter}";
        }

        public static string RegistrationAdmin(string to, string token)
        {
            return $"registration/admin?email={System.Net.WebUtility.UrlEncode(to)}&token={System.Net.WebUtility.UrlEncode(token)}";
        }

        public static string ResetPassword(string to, string token, string lang = null)
        {
            var langParameter = (lang != null) ? $"&lang={lang}" : "";
            return $"reinitialiser-mot-de-passe?email={System.Net.WebUtility.UrlEncode(to)}&token={System.Net.WebUtility.UrlEncode(token)}{langParameter}";
        }
    }
}
