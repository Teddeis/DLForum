using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("comments")]
public class comment : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }
    public int id_users { get; set; }
    public int id_topics { get; set; }
    public string author { get; set; }
    public string avatar {  get; set; }
    public string comments { get; set; }
}