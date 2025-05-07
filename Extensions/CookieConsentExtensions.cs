using DLForum.Middleware;
using Microsoft.AspNetCore.Builder;

namespace DLForum.Extensions
{
    public static class CookieConsentExtensions
    {
        public static IApplicationBuilder UseCookieConsent(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieConsentMiddleware>();
        }
    }
} 