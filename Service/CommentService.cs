using Supabase;
using Supabase.Interfaces;
using DLForum.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DLForum.Models.Comments;

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

    // Метод для получения комментариев по теме
    public async Task<List<comment>> GetCommentsByTopicIdAsync(int topicId)
    {
        // Загружаем комментарии
        var commentResponse = await _client
            .From<comment>()
            .Select("id, comments, id_users, created, parent_id")
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

    // Метод для добавления комментария
    public async Task<report?> AddReportAsync(int userId, string owr, string content)
    {
        var reports = new report
        {
            id_users = userId,
            owr = owr,
            reason = content,
            created = DateTime.Now,
        };

        var response = await _client.From<report>().Insert(reports);

        return response.Models.FirstOrDefault();
    }


    // Получить все ответы на конкретный комментарий
    public async Task<List<comment>> GetRepliesAsync(int parentId)
    {
        var response = await _client
            .From<comment>()
            .Select("id, comments, id_users, created, parent_id")
            .Filter("parent_id", Supabase.Postgrest.Constants.Operator.Equals, parentId)
            .Order("created", Supabase.Postgrest.Constants.Ordering.Ascending)
            .Get();

        var replies = response.Models.ToList();

        // Подгружаем пользователей для ответов
        var userIds = replies.Select(c => c.id_users).Distinct().ToList();
        if (userIds.Any())
        {
            var userResponse = await _client
                .From<users>()
                .Select("id, username, avatar_url, role")
                .Filter("id", Supabase.Postgrest.Constants.Operator.In, userIds)
                .Get();

            var usersDict = userResponse.Models.ToDictionary(u => u.id);
            foreach (var reply in replies)
            {
                if (usersDict.TryGetValue(reply.id_users, out var user))
                    reply.users = user;
            }
        }

        return replies;
    }

    // Добавить ответ на комментарий
    public async Task<createcomments?> AddReplyAsync(int userId, int topicId, string content, int parentId)
    {
        var reply = new createcomments
        {
            id_users = userId,
            id_topics = topicId,
            comments = content,
            created = DateTime.Now,
            parent_id = parentId
        };

        var response = await _client.From<createcomments>().Insert(reply);
        return response.Models.FirstOrDefault();
    }

}