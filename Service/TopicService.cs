using Supabase;
using Supabase.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class TopicService
{
    private readonly Client _client;

    public TopicService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    // Метод для добавления новой записи в таблицу "topics"
    public async Task<Topic?> AddTopicAsync(int userId, string title, string content)
    {
        var topic = new Topic
        {
            UserId = userId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ViewsCount = 0,
            LikesCount = 0
        };

        var response = await _client.From<Topic>().Insert(topic);

        return response.Models.FirstOrDefault();
    }

    // Метод для получения всех записей с пагинацией
    public async Task<(List<Topic>, int)> GetTopicsByPageAsync(int pageNumber, int pageSize)
    {
        Topic topic = new Topic();
        // Рассчитываем, сколько тем пропустить
        var offset = (pageNumber - 1) * pageSize;

        // Получаем темы с учетом нагинации
        var response = await _client.From<Topic>()
            .Limit(pageSize)
            .Offset(offset).
            Where(x => topic.Status == "true")
            .Get();

        // Получаем общее количество тем
        var totalCountResponse = await _client.From<Topic>().Get();

        // Возвращаем список тем и общее количество для пагинации
        return (response.Models.ToList(), totalCountResponse.Models.Count);
    }

    // Метод для получения всех записей с пагинацией
    public async Task<(List<Topic>, int)> GetTopicsByPageAsyncAdmin(int pageNumber, int pageSize)
    {
        Topic topic = new Topic();
        // Рассчитываем, сколько тем пропустить
        var offset = (pageNumber - 1) * pageSize;

        // Получаем темы с учетом нагинации
        var response = await _client.From<Topic>()
            .Limit(pageSize)
            .Offset(offset).
            Where(x => topic.Status == "false")
            .Get();

        // Получаем общее количество тем
        var totalCountResponse = await _client.From<Topic>().Get();

        // Возвращаем список тем и общее количество для пагинации
        return (response.Models.ToList(), totalCountResponse.Models.Count);
    }
}
