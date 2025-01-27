using Supabase;


public class TopicService
{
    private readonly Client _client;


    public TopicService(SupabaseClientService clientService)
    {
        _client = clientService.Client;

    }

    // Метод для добавления новой записи в таблицу "topics"
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

    // Метод для обновления статуса темы
    public async Task<Topic?> UpdateTopicStatusAsync(int topicId, bool newStatus)
    {
        // Ищем тему по ID
        var topic = await _client.From<Topic>().Where(t => t.Id == topicId).Get();

        // Если тема не найдена
        if (!topic.Models.Any())
        {
            return null;
        }

        var topicToUpdate = topic.Models.First();
        topicToUpdate.Status = newStatus.ToString(); // Преобразуем булево значение в строку

        // Обновляем тему в базе данных
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Update(topicToUpdate);

        return response.Models.FirstOrDefault(); // Возвращаем обновленную тему
    }


    // Метод для обновления статуса темы
    public async Task<Topic?> DeleteTopicStatusAsync(int topicId, bool newStatus)
    {
        // Ищем тему по ID
        var topic = await _client.From<Topic>().Where(t => t.Id == topicId).Get();

        // Если тема не найдена
        if (!topic.Models.Any())
        {
            return null;
        }

        var topicToUpdate = topic.Models.First();
        topicToUpdate.Status = newStatus.ToString(); // Преобразуем булево значение в строку

        // Обновляем тему в базе данных
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Delete(topicToUpdate);

        return response.Models.FirstOrDefault(); // Возвращаем обновленную тему
    }


    // Метод для обновления статуса темы
    public async Task<Topic?> Delete(int topicId)
    {
        // Ищем тему по ID
        var topic = await _client.From<Topic>().Where(t => t.Id == topicId).Get();

        // Если тема не найдена
        if (!topic.Models.Any())
        {
            return null;
        }

        var topicToUpdate = topic.Models.First();
        // Обновляем тему в базе данных
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Delete(topicToUpdate);

        return response.Models.FirstOrDefault(); // Возвращаем обновленную тему
    }    
}