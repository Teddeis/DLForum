using DLForum.Models.Topic;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DLForum.Service
{
    public class ImageService
    {
        private readonly Supabase.Client _client;

        public ImageService(SupabaseClientService clientService)
        {
            _client = clientService.Client;
        }

        public async Task<List<images>> GetImagesByTopicIdAsync(int topicId)
        {
            try
            {
                // Выполняем запрос к базе данных, чтобы получить изображения по идентификатору темы
                var response = await _client.From<images>().Where(x => x.TopicId == topicId).Get();

                return response.Models.ToList(); // Возвращаем список изображений
            }
            catch (Exception ex)
            {
                // В случае ошибки возвращаем пустой список и записываем ошибку
                // (или можно сделать логирование)
                Console.WriteLine("Ошибка: " + ex.Message);
                return new List<images>(); // Возвращаем пустой список в случае ошибки
            }
        }
    }
}
