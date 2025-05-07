using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DLForum.Middleware
{
    public class CookieConsentMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieConsentMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Если куки согласия нет, просто пропускаем
            if (!context.Request.Cookies.ContainsKey("CookieConsent"))
            {
                await _next(context);
                return;
            }

            // Получаем значение согласия
            var consent = context.Request.Cookies["CookieConsent"];

            // Если согласие отклонено, удаляем только аналитические и рекламные куки
            if (consent == "false")
            {
                var cookiesToRemove = context.Request.Cookies.Keys
                    .Where(cookie => cookie.StartsWith("analytics_") || 
                                   cookie.StartsWith("advertising_") ||
                                   cookie == "visitor_id" ||
                                   cookie == "tracking_id");

                foreach (var cookie in cookiesToRemove)
                {
                    context.Response.Cookies.Delete(cookie);
                }
            }

            await _next(context);
        }
    }
} 