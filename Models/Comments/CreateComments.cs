using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("comments")]
public class createcomments : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    public int id_users { get; set; }
    public int id_topics { get; set; }
    public string comments { get; set; }
    public DateTime created { get; set; }
    public int? parent_id { get; set; }

}