using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace DLForum.Models
{
    [Table("image")]
    public class images : BaseModel
    {
        [PrimaryKey("id", false)] // Убедитесь, что у вас есть поле `id` в таблице, которое является первичным ключом
        public int Id { get; set; }

        [Column("topic_id")]
        public int TopicId { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }
    }
}
