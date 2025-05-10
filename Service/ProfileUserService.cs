using System.Xml.Linq;
using AllTopics;
using DLForum;
using DLForum.Models.Topic;
using DLForum.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Exceptions;

public class ProfileUserService
{
    private readonly Supabase.Client _client;

    public ProfileUserService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    public async Task<bool> EmailExistsAsync(string email, int excludeUserId)
    {
        var users = await _client.From<users>().Where(u => u.email == email && u.id != excludeUserId).Get();
        return users.Models.Any();
    }

    public async Task<users?> GetUserByIdAsync(int id)
    {
        var response = await _client.From<users>().Where(x => x.id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<users?> UpdateUserAsync(users user)
    {
        try
        {
            var response = await _client
                .From<users>()
                .Where(x => x.id == user.id)
                .Update(user);

            return response.Models.FirstOrDefault();
        }
        catch (PostgrestException ex)
        {
            Console.Error.WriteLine($"Postgrest error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            throw;
        }
    }

    public async Task<List<Topic>> GetTopicsByAuthorAsync(int author)
    {
        var response = await _client.From<Topic>()
            .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<Topic>();
    }

    public async Task<List<comment>> GetCommentsByAuthorAsync(int author)
    {
        var response = await _client.From<comment>()
            .Select("id, id_topics, comments, users(id, username, avatar_url)")  // Включаем данные из таблицы users
            .Filter("id_users", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<comment>();
    }


    public async Task<List<Topic>> GetTopicsByIdAsync(int author)
    {
        var response = await _client.From<Topic>()
            .Where(t => t.Status == "true")
            .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<Topic>();
    }

    public async Task<List<comment>> GetCommentsByIdAsync(int author)
    {
        var response = await _client
            .From<comment>()
            .Select("id, id_topics, comments, users(id, username, avatar_url)")
            .Filter("id_users", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<comment>();
    }

    public async Task<List<favoritelist>> GetFavorite(int id)
    {
        var response = await _client
            .From<favoritelist>()
            .Select("id,id_user,id_topics, topics(id, title,created_at)")
            .Filter("id_user", Supabase.Postgrest.Constants.Operator.Equals, id)
            .Get();

        return response.Models ?? new List<favoritelist>();
    }



    public async Task<UserProfileDto> GetUserProfileDataAsync(int userId)
    {
        var user = await _client.From<users>()
            .Where(u => u.id == userId)
            .Single();

        if (user == null)
            return null;

        var topicsTask = _client.From<Topic>().Where(t => t.UserId == userId).Get();
        var commentsTask = _client.From<comment>().Where(c => c.id_users == userId).Get();
        var favoritesTask = _client.From<favorite>().Where(f => f.UserId == userId).Get();

        await Task.WhenAll(topicsTask, commentsTask, favoritesTask);

        return new UserProfileDto
        {
            User = user,
            Topics = topicsTask.Result.Models.ToList(),     // Достаем реальные данные
            Comments = commentsTask.Result.Models.ToList(),
            Favorites = favoritesTask.Result.Models.ToList()
        };
    }

    public async Task<List<DLForum.Models.Comments.report>> GetReportsAsync()
    {
        var response = await _client.From<DLForum.Models.Comments.report>().Get();
        return response.Models ?? new List<DLForum.Models.Comments.report>();
    }

    public async Task<List<users>> GetAllUsersAsync()
    {
        var response = await _client.From<users>().Get();
        return response.Models ?? new List<users>();
    }

    public async Task<comment?> GetCommentByIdAsync(int commentId)
    {
        var response = await _client.From<comment>()
            .Where(c => c.Id == commentId)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        await _client.From<comment>()
            .Where(c => c.Id == commentId)
            .Delete();
    }

    public async Task<Topic?> GetTopicByIdAsync(int topicId)
    {
        var response = await _client.From<Topic>()
            .Where(t => t.Id == topicId)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public async Task DeleteTopicAsync(int topicId)
    {
        await _client.From<Topic>()
            .Where(t => t.Id == topicId)
            .Delete();
    }

    public async Task ResolveReportAsync(int reportId)
    {
        await _client.From<report>()
            .Where(r => r.Id == reportId)
            .Delete();
    }

    public async Task<List<Topic>> GetAllTopicsAsync()
    {
        var response = await _client.From<Topic>().Get();
        return response.Models ?? new List<Topic>();
    }

    public async Task<report?> GetReportByIdAsync(int reportId)
    {
        var response = await _client.From<report>()
            .Where(r => r.Id == reportId)
            .Get();
        return response.Models.FirstOrDefault();
    }

    public class UserProfileDto
    {
        public users User { get; set; }
        public List<Topic> Topics { get; set; }
        public List<comment> Comments { get; set; }
        public List<favorite> Favorites { get; set; }
    }

}