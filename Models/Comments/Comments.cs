using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("comments")]
public class comment : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    public int id_users { get; set; }
    public int id_topics { get; set; }
    public string comments { get; set; }
    public DateTime created { get; set; }
    public users users { get; set; } // Навигационное свойство

}