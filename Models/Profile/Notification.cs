using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Profile
{
    [Table("notification")]
    public class notification : BaseModel
    {
        [PrimaryKey] // Указываем, что это первичный ключ
        public int id { get; set; } // Объявляем id как первичный ключ

        public int id_users { get; set; }
        public string message { get; set; }
        public string from_to { get; set; }
        public bool read { get; set; } = false;
        public DateTime created_at { get; set; }
    }
}
