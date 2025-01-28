namespace DLForum.Models
{
    public class Profile
    {
        public users user { get; set; } // Пользователь
        public List<Topic> TopicsList { get; set; } // Темы пользователя
        public List<comment> CommentsList { get; set; } // Комментарии пользователя
    }
}
