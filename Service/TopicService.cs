using DLForum.Models;
using Supabase;

public class TopicService
{
    private readonly Client _client;

    public TopicService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    // Добавление темы
    public async Task<Topic?> AddTopicAsync(int userId, string title, string content, string author, string categories, string tags)
    {
        var topic = new Topic
        {
            UserId = userId,
            Title = title,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            LikesCount = 0,
            Status = "false",
            Author = author,
            Categories = categories,
            Tags = tags
        };

        var response = await _client.From<Topic>().Insert(topic);
        return response.Models.FirstOrDefault();
    }

    // Получение тем с пагинацией (только одобренные)
    public async Task<(List<Topic>, int)> GetTopicsByPageAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        var response = await _client.From<Topic>()
            .Where(t => t.Status == "true")
            .Limit(pageSize)
            .Offset(offset)
            .Get();

        var totalCount = (await _client.From<Topic>().Where(t => t.Status == "true").Get()).Models.Count;

        return (response.Models.ToList(), totalCount);
    }


    
    public async Task<(List<Topic>, int)> GetTopicsByPageCategoriesAsync(string categories, int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        var query = _client.From<Topic>().Where(t => t.Status == "true");

        // Фильтрация по категории
        if (!string.IsNullOrEmpty(categories))
        {
            query = query.Where(t => t.Categories.Contains(categories));
        }

        // Получаем отфильтрованные темы с учетом пагинации
        var response = await query
            .Limit(pageSize)
            .Offset(offset)
            .Get();

        // Получаем общее количество записей без учета пагинации
        var totalCount = (await query.Get()).Models.Count;

        return (response.Models.ToList(), totalCount);
    }



    // Получение тем с пагинацией для админов (неодобренные)
    public async Task<(List<Topic>, int)> GetTopicsByPageAsyncAdmin(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        var response = await _client.From<Topic>()
            .Where(t => t.Status == "false")
            .Limit(pageSize)
            .Offset(offset)
            .Get();

        var totalCount = (await _client.From<Topic>().Where(t => t.Status == "false").Get()).Models.Count;

        return (response.Models.ToList(), totalCount);
    }

    // Обновление статуса темы
    public async Task<Topic?> UpdateTopicStatusAsync(int topicId, bool newStatus)
    {
        var topic = await GetTopicByIdAsync(topicId);
        if (topic == null) return null;

        topic.Status = newStatus.ToString();
        await _client.From<Topic>().Where(t => t.Id == topic.Id).Update(topic);

        return topic;
    }

    // Удаление темы
    public async Task<bool> DeleteTopicAsync(int topicId)
    {
        var topic = await GetTopicByIdAsync(topicId);
        if (topic == null) return false;

        await _client.From<Topic>().Where(t => t.Id == topic.Id).Delete();
        return true;
    }

    // Получение темы по ID
    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Get();
        return response.Models.FirstOrDefault();
    }
}
