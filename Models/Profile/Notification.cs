using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Profile
{
    [Table("notification")]
    public class notification : BaseModel
    {
        [PrimaryKey] // Указываем, что это первичный ключ
        public int id { get; set; } // Объявляем id как первичный ключ

        [Column("id_users")]
        public int id_users { get; set; }

        [Column("message")]
        public string message { get; set; }

        [Column("from_to")]
        public string from_to { get; set; }

        [Column("read")]
        public bool read { get; set; } = false;

        [Column("created_at")]
        public DateTime created_at { get; set; }

        [Column("type")]
        public string type { get; set; }
    }
}
