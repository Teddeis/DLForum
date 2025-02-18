using AllTopics;

namespace Profile
{
    public class Profile
    {
        public users user { get; set; } // Пользователь
        public List<Topic> TopicsList { get; set; } // Темы пользователя
        public List<comment> CommentsList { get; set; } // Комментарии пользователя
        public List<favoritelist> FavoritesList { get; set; }
    }
}
