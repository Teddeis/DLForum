using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

public class users : BaseModel
{
    [PrimaryKey("id", false)] // Укажите true для автоинкрементного ключа
    public int Id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public string role { get; set; } = "user";
    public bool is_active { get; set; } = true;
    public string avatar_url { get; set; } = "https://grandfestival.vcht.center/storage/exponents/logos/2186_logo.jpg";
}
