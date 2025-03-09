using DLForum.Models.Topic;
using Supabase;
using Supabase.Interfaces;
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
                .Single();

            return favorite != null;
        }

        public async Task RemoveFavorite(int userId, int topicId)
        {
            await _client.From<favorite>().Where(x => x.TopicId == topicId && x.UserId == userId)
                .Delete();

        }

        public async Task<bool> IsLike(int userId, int topicId)
        {
            var like = await _client.From<like>().Where(x => x.TopicId == topicId && x.UserId == userId)
                .Single();

            return like != null;
        }

        public async Task RemoveLike(int userId, int topicId)
        {
            await _client.From<like>().Where(x => x.TopicId == topicId && x.UserId == userId)
                .Delete();
        }

        public async Task<int> GetLikeCount(int topicId)
        {
            var likes = await _client
                .From<like>()
                .Where(l => l.TopicId == topicId)
                .Get();

            return likes.Models.Count;
        }

        public async Task<int> GetCommentCount(int topicId)
        {
            var likes = await _client
                .From<comment>()
                .Where(l => l.id_topics == topicId)
                .Get();

            return likes.Models.Count;
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

        // Лайк
        public async Task<like?> AddLike(int userId, int topicId)
        {
            var newLike = new like
            {
                UserId = userId,
                TopicId = topicId,
            };

            var response = await _client.From<like>().Insert(newLike);

            return response.Models.FirstOrDefault();
        }
    }
}
