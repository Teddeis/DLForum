using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DLForum.Models.Comments
{
    [Table("report")]
    public class report : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        public int id_users { get; set; }
        public string owr { get; set; }
        public string reason { get; set; }
        public DateTime created { get; set; }
    }
}
