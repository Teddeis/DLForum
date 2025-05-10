using NPOI.Util;
using Profile;
using Supabase;

namespace DLForum.Service
{
    public class NotificationService
    {
        private readonly Client _client;

        public NotificationService(SupabaseClientService clientService)
        {
            _client = clientService.Client;
        }

        public async Task CreateNotificationAsync(int userId, string message, string fromTo, string type)
        {
            var notification = new notification
            {
                id_users = userId,
                message = message,
                from_to = fromTo,
                type = type,
                created_at = DateTime.UtcNow
            };

            await _client.From<notification>().Insert(notification);
        }

        public async Task<List<notification>> GetUserNotificationsAsync(int userId)
        {
            var response = await _client.From<notification>()
                .Where(n => n.id_users == userId)
                .Order(n => n.created_at, Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            return response.Models ?? new List<notification>();
        }

        public async Task MarkNotificationAsReadAsync(int notificationId)
        {
            var notification = new notification
            {
                id = notificationId,
                read = true
            };

            await _client.From<notification>()
                .Where(n => n.id == notificationId)
                .Update(notification);
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

        public async Task<List<notification>> GetSystemNotificationsAsync()
        {
            var response = await _client.From<notification>()
                .Where(n => n.from_to == "system")
                .Order(n => n.created_at, Supabase.Postgrest.Constants.Ordering.Descending)
                .Limit(10)
                .Get();

            return response.Models ?? new List<notification>();
        }
    }
}