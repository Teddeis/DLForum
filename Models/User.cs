using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("users")]
public class users : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    public string username { get; set; } = GenerateRandomUsername();

    public string email { get; set; }
    public string password_hash { get; set; }
    public string role { get; set; } = "user";
    public bool is_active { get; set; } = true;
    public string gender { get; set; } = "Не указан";
    public string about { get; set; } = "Пусто";
    public string avatar_url { get; set; } = "https://grandfestival.vcht.center/storage/exponents/logos/2186_logo.jpg";
    public string you_background { get; set; } = "Пусто";


    private static Random _random = new Random();
    private static string GenerateRandomUsername(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}
