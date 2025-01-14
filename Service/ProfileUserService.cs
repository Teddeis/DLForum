using Supabase;

public class ProfileUserService
{
    private readonly Supabase.Client _client;

    public ProfileUserService(SupabaseClientService clientService)
    {
        _client = clientService.Client; 
    }

    public async Task<users?> GetUserByIdAsync(int id)
    {
        var response = await _client.From<users>().Where(x => x.Id == id).Get();
        return response.Models.FirstOrDefault();
    }
}
