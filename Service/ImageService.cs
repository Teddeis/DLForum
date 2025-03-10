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
                var response = await _client.From<images>().Where(x => x.TopicId == topicId).Get();

                return response.Models.ToList(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
                return new List<images>(); 
            }
        }
    }
}
