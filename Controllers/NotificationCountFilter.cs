using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class NotificationCountFilter : IAsyncActionFilter
{
    private readonly NotificationService _notificationService;

    public NotificationCountFilter(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Получаем ID текущего пользователя из сессии
        int? currentUserId = context.HttpContext.Session.GetInt32("ID");

        if (currentUserId != null)
        {
            var notifications = await _notificationService.GetUserNotificationsAsyncReadFalse(currentUserId.Value);
            context.HttpContext.Items["NotificationCount"] = notifications.Count;
        }
        else
        {
            context.HttpContext.Items["NotificationCount"] = 0;
        }

        await next();
    }
}
