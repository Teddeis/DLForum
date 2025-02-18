using DLForum.Models.Topic;
using DLForum.Service;
using Microsoft.AspNetCore.Mvc;
using Supabase;

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
    }
}
