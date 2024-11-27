namespace DLForum.Models
{
    public class TopicViewModel
    {
        public Topic Topic { get; set; }
        public List<comment> Comments { get; set; } // Новый список комментариев
    }
}
