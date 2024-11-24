using Supabase.Postgrest.Models;

public class users : BaseModel
{
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public string role { get; set; } = "user";
    public bool is_active { get; set; } = true;
    public string avatar_url { get; set; }
}
