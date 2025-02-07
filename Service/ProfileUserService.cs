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

    public async Task<users?> GetUserByIdAsync(int id)
    {
        var response = await _client.From<users>().Where(x => x.id == id).Get();
        return response.Models.FirstOrDefault();
    }

    public async Task<users?> UpdateUserAsync(users user)
    {
        try
        {
            // Perform the update operation
            var response = await _client
                .From<users>()
                .Where(x => x.id == user.id) // Filter by ID
                .Update(user);

            // Return the updated user or null if no update occurred
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


    // Получение списка тем пользователя
    public async Task<List<Topic>> GetTopicsByAuthorAsync(string author)
    {
        var response = await _client.From<Topic>()
            .Filter("author", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<Topic>();
    }

    // Получение списка комментариев пользователя
    public async Task<List<comment>> GetCommentsByAuthorAsync(string author)
    {
        var response = await _client.From<comment>()
            .Filter("author", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<comment>();
    }


    // Получение списка тем пользователя
    public async Task<List<Topic>> GetTopicsByIdAsync(int author)
    {
        var response = await _client.From<Topic>()
            .Where(t => t.Status == "true")
            .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<Topic>();
    }

    // Получение списка комментариев пользователя
    public async Task<List<comment>> GetCommentsByIdAsync(int author)
    {
        var response = await _client.From<comment>()
            .Filter("id_users", Supabase.Postgrest.Constants.Operator.Equals, author)
            .Get();

        return response.Models ?? new List<comment>();
    }
}
