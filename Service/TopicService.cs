﻿using System.Linq;
using DLForum.Models;
using DLForum.Models.Topic;
using Supabase;
using static Supabase.Postgrest.Constants;

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
            CreatedAt = DateTime.Now,
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
    public async Task<(List<Topic>, int)> GetTopicsByPageAsync(int pageNumber, int pageSize, string sortOrder = "newest")
    {
        var offset = (pageNumber - 1) * pageSize;

        var query = _client.From<Topic>()
            .Where(t => t.Status == "true");

        query = sortOrder switch
        {
            "newest" => query.Order(t => t.CreatedAt, Ordering.Descending),
            "oldest" => query.Order(t => t.CreatedAt, Ordering.Ascending),
            _ => query.Order(t => t.CreatedAt, Ordering.Descending)
        };

        var response = await query
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

        if (!string.IsNullOrEmpty(categories))
        {
            query = query.Where(t => t.Categories.Contains(categories));
        }

        var response = await query
            .Limit(pageSize)
            .Offset(offset)
            .Get();

        var totalCount = (await query.Get()).Models.Count;

        return (response.Models.ToList(), totalCount);
    }

    // Получение тем с пагинацией для Популярное (только одобренные)
    public async Task<(List<Topic>, int)> GetTopicsByPagePopularAsync(int pageNumber, int pageSize)
    {
        var offset = (pageNumber - 1) * pageSize;

        var response = await _client.From<Topic>()
            .Where(t => t.Status == "true" && t.LikesCount != 0)
            .Limit(pageSize)
            .Offset(offset)
            .Order(t => t.LikesCount, Ordering.Descending)
            .Get();

        var totalCount = (await _client.From<Topic>().Where(t => t.Status == "true" && t.LikesCount != 0).Order(t => t.LikesCount, Ordering.Descending).Get()).Models.Count;

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

    // Метод для обновления статуса темы
    public async Task<Topic?> DeleteTopicStatusAsync(int topicId, bool newStatus)
    {
        var topic = await _client.From<Topic>().Where(t => t.Id == topicId).Get();

        if (!topic.Models.Any())
        {
            return null;
        }

        var topicToUpdate = topic.Models.First();
        topicToUpdate.Status = newStatus.ToString();

        // Обновляем тему в базе данных
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Delete(topicToUpdate);

        return response.Models.FirstOrDefault();
    }

    // Удаление темы
    public async Task<bool> DeleteTopicAsync(int topicId)
    {
        var topic = await GetTopicByIdAsync(topicId);
        if (topic == null) return false;

        await _client.From<like>()
                     .Where(c => c.TopicId == topic.Id)
                     .Delete();

        await _client.From<comment>()
                     .Where(c => c.id_topics == topic.Id)
                     .Delete();

        await _client.From<favorite>()
                     .Where(c => c.TopicId == topic.Id)
                     .Delete();

        await _client.From<Topic>()
                     .Where(t => t.Id == topic.Id)
                     .Delete();

        return true;
    }

    // Получение темы по ID
    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        var response = await _client.From<Topic>().Where(t => t.Id == topicId).Get();
        return response.Models.FirstOrDefault();
    }
}