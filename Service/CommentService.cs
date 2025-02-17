using Supabase;
using Supabase.Interfaces;
using DLForum.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommentService
{
    private readonly Client _client;

    public CommentService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    // Метод для добавления комментария
    public async Task<createcomments?> AddCommentAsync(int userId, int topicId, string content)
    {
        var comment = new createcomments
        {
            id_users = userId,
            id_topics = topicId,
            comments = content,
            created = DateTime.Now,
        };

        var response = await _client.From<createcomments>().Insert(comment);

        return response.Models.FirstOrDefault();
    }

    // Метод для добавления ответа на комментарий
    public async Task<branch?> AddReplyAsync(int userId, int parentId, string content)
    {
        var reply = new branch
        {
            id_users = userId,
            comments = content,
            id_comment = parentId, // Указываем родительский комментарий
        };

        var response = await _client.From<branch>().Insert(reply);

        return response.Models.FirstOrDefault();
    }

    // Метод для получения комментариев по теме
    public async Task<List<comment>> GetCommentsByTopicIdAsync(int topicId)
    {
        // Загружаем комментарии
        var commentResponse = await _client
            .From<comment>()
            .Select("id, comments, id_users, created")
            .Filter("id_topics", Supabase.Postgrest.Constants.Operator.Equals, topicId)
            .Get();

        var comments = commentResponse.Models.ToList();

        // Собираем все id_users для запроса пользователей
        var userIds = comments.Select(c => c.id_users).Distinct().ToList();

        if (userIds.Any())
        {
            // Загружаем пользователей
            var userResponse = await _client
                .From<users>()
                .Select("id, username, avatar_url,role")
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, userIds)
                .Get();

            var users = userResponse.Models.ToDictionary(u => u.id);

            // Добавляем данные пользователей в комментарии
            foreach (var comment in comments)
            {
                if (users.TryGetValue(comment.id_users, out var user))
                {
                    comment.users = user;
                }
            }
        }

        return comments;
    }

    // Метод для получения комментариев и их ответов по теме
    public async Task<List<branch>> GetReplayCommentsByTopicIdAsync(int topicId)
    {
        // Загружаем комментарии
        var commentResponse = await _client
            .From<branch>()
            .Select("id, comments, id_users")
            .Filter("id_topics", Supabase.Postgrest.Constants.Operator.Equals, topicId)
            .Get();

        var comments = commentResponse.Models.ToList();

        // Собираем все id_users для запроса пользователей
        var userIds = comments.Select(c => c.id_users).Distinct().ToList();

        if (userIds.Any())
        {
            // Загружаем пользователей
            var userResponse = await _client
                .From<users>()
                .Select("id, username, avatar_url")
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, userIds)
                .Get();

            var users = userResponse.Models.ToDictionary(u => u.id);

            // Добавляем данные пользователей в комментарии
            foreach (var comment in comments)
            {
                if (users.TryGetValue(comment.id_users, out var user))
                {
                    comment.users = user;
                }
            }
        }

        return comments;
    }

}
