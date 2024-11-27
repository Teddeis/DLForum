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
    public async Task<comment?> AddCommentAsync(int userId, int topicId, string author, string content)
    {
        var comment = new comment
        {
            id_users = userId,
            id_topics = topicId,
            author = author,
            comments = content,
        };

        var response = await _client.From<comment>().Insert(comment);

        return response.Models.FirstOrDefault();
    }

    // Метод для получения комментариев по теме
    public async Task<List<comment>> GetCommentsByTopicIdAsync(int topicId)
    {
        var response = await _client.From<comment>().Where(x => x.id_topics == topicId).Get();
        return response.Models.ToList();
    }


}
