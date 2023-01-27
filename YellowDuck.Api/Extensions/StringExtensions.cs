using System;
using System.Text;

namespace YellowDuck.Api.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64(this string value)
        {
            var bytes = Encoding.Default.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string Base64Decode(this string value)
        {
            var bytes = Convert.FromBase64String(value);
            return Encoding.Default.GetString(bytes);
        }
    }
}