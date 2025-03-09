using DLForum.Models.Topic;
using DLForum.Service;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using Supabase.Interfaces;

namespace DLForum.Service
{
    public class DetailsService
    {
        private readonly Client _client;


        public DetailsService(SupabaseClientService clientService)
        {
            _client = clientService.Client;

        }
        public async Task<Topic?> GetTopicByIdAsync(int id)
        {
            var response = await _client.From<Topic>().Where(x => x.Id == id).Get();
            return response.Models.FirstOrDefault();
        }

        public async Task UpdateLikesCount(int topicId, int likeCount)
        {
            await _client
                .From<Topic>()
                .Where(t => t.Id == topicId)
                .Set(t => t.LikesCount, likeCount) // Обновляем только `LikesCount`
                .Update();
        }

        public async Task UpdateCommentCount(int topicId, int commentCount)
        {
            await _client
                .From<Topic>()
                .Where(t => t.Id == topicId)
                .Set(t => t.CommentsCount, commentCount) // Обновляем только `CommentsCount`
                .Update();
        }
    }
}
