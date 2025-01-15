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
}
