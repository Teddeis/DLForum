using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("branch")]
public class branch : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    public int id_topics { get; set; }

    // Используем id_comment как связь с родительским комментарием
    // Если это корневой комментарий, то id_comment будет null
    public int? id_comment { get; set; }

    public int id_users { get; set; }
    public string comments { get; set; }
    public users users { get; set; }
}
