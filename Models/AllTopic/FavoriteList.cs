using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AllTopics

{
    [Table("favorite")]
    public class favoritelist : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("id_topics")]
        public int TopicId { get; set; }

        [Column("id_user")]
        public int UserId { get; set; }

        public Topic topics { get; set; }
    }
}

