using Supabase;
using DLForum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Supabase.Postgrest.Constants;

public class SearchService
{
    private readonly Client _client;

    public SearchService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    public async Task<List<object>> GetSuggestionsAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Query не может быть пустым");
        }

        try
        {
            // Поиск тем с использованием оператора Like для каждого поля отдельно
            var topicsResponse = await _client.From<Topic>()
                .Filter(t => t.Title, Operator.Like, $"%{query}%")
                .Select("id, title")
                .Get();

            var topics = topicsResponse.Models
                .Select(t => t.Title)
                .ToList();

            // Поиск тем по тегам
            var topicsTagsResponse = await _client.From<Topic>()
                .Filter(t => t.Tags, Operator.Like, $"%{query}%")
                .Select("id, title")
                .Get();

            var topicsTags = topicsTagsResponse.Models
                .Select(t => t.Title)
                .ToList();

            // Поиск тем по категориям
            var topicsCategoriesResponse = await _client.From<Topic>()
                .Filter(t => t.Categories, Operator.Like, $"%{query}%")
                .Select("id, title")
                .Get();

            var topicsCategories = topicsCategoriesResponse.Models
                .Select(t => t.Title)
                .ToList();

            // Объединяем все найденные темы
            var allTopics = topics.Concat(topicsTags).Concat(topicsCategories).ToList();

            // Поиск пользователей с использованием оператора Like
            var usersResponse = await _client.From<users>()
                .Filter(u => u.username, Operator.Like, $"%{query}%")
                .Select("id, username, avatar_url")
                .Get();

            var users = usersResponse.Models
                .Select(u => u.username)
                .ToList();

            // Объединяем все списки и возвращаем
            return allTopics.Concat(users).ToList<object>();
        }
        catch (Exception ex)
        {
            throw new Exception("Ошибка при поиске данных в Supabase", ex);
        }
    }
}

