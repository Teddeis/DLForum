using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DLForum.Models.Topic
{
    [Table("favorite")]
    public class favorite : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("id_topics")]
        public int TopicId { get; set; }

        [Column("id_user")]
        public int UserId { get; set; }
    }
}
