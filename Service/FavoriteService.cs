using DLForum.Models.Topic;
using Supabase;
using Supabase.Postgrest.Exceptions;

namespace DLForum.Service
{
    public class FavoriteService
    {
        private readonly Client _client;


        public FavoriteService(SupabaseClientService clientService)
        {
            _client = clientService.Client;

        }

        public async Task<bool> IsFavorite(int userId, int topicId)
        {
            var favorite = await _client.From<favorite>().Where(x=> x.TopicId == topicId && x.UserId == userId)
                .Single(); // Для получения одного объекта, так как это фильтрация по уникальной паре (userId, topicId)

            return favorite != null;
        }

        public async Task RemoveFavorite(int userId, int topicId)
        {
            await _client.From<favorite>().Where(x => x.TopicId == topicId && x.UserId == userId)
                .Delete();

        }

        // Добавление в избранное
        public async Task<favorite?> AddFavorite(int userId, int topicId)
        {
            var newFavorite = new favorite
            {
                UserId = userId,
                TopicId = topicId,
            };

            var response = await _client.From<favorite>().Insert(newFavorite);

            return response.Models.FirstOrDefault();
        }

    }
}
