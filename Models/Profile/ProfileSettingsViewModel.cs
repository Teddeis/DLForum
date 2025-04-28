using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Microsoft.AspNetCore.Http;

namespace Profile
{
    [Table("users")]
    public class ProfileSettingsViewModel
    {
        public string avatar_url { get; set; }
        public string you_background { get; set; }
        public string username { get; set; } = string.Empty;
        public string gender { get; set; } = "Не указан";
        public string about { get; set; } = "Пусто";
        public IFormFile avatar { get; set; }
        public IFormFile banner { get; set; }
        public string croppedAvatar { get; set; }
        public string croppedBanner { get; set; }
    }

}
