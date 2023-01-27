using YellowDuck.Api.Utilities;
using System;
using System.Globalization;

namespace YellowDuck.Api.Extensions
{
    public static class CultureExtensions
    {
        public static IDisposable Substitute(this CultureInfo culture) => new CultureSubstitution(culture);
    }
}
