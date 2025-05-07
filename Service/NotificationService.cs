using System.Reactive;
using AllTopics;
using Microsoft.EntityFrameworkCore;
using Profile;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Exceptions;

public class NotificationService
{
    private readonly Supabase.Client _client;

    public NotificationService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    public async Task<List<notification>> GetUserNotificationsAsync(int id)
    {
        var response = await _client.From<notification>().Where(x => x.id_users == id).Get();
        return response.Models ?? new List<notification>();
    }

    public async Task<List<notification>> GetUserNotificationsAsyncReadFalse(int id)
    {
        var response = await _client.From<notification>().Where(x => x.id_users == id && x.read == false).Get();
        return response.Models ?? new List<notification>();
    }

    public async Task<notification?> GetNotificationByIdAsync(int notificationId)
    {
        var response = await _client.From<notification>().Where(x => x.id == notificationId).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task UpdateNotificationAsync(notification notification)
    {
        await _client.From<notification>().Update(notification);
    }


    public async Task<notification?> AddNotificationAsync(int userId, string message, string fromTo, bool read, DateTime created_at, string type)
    {
        var notification = new notification
        {
            id_users = userId,
            message = message,
            from_to = fromTo,
            read = read,
            created_at = created_at,
            type = type
        };

        var response = await _client.From<notification>().Insert(notification);
        return response.Models.FirstOrDefault();
    }
}