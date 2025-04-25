using DLForum.Models;

namespace DLForum.Service
{
    public interface ITopicService
    {
        Task<(List<Topic>, int)> GetTopicsByPageAsync(int pageNumber, int pageSize, string sortOrder = "newest");
        Task<(List<Topic>, int)> GetTopicsByPageCategoriesAsync(string category, int pageNumber, int pageSize);
        Task<(List<Topic>, int)> GetTopicsByPagePopularAsync(int pageNumber, int pageSize);
        Task<Topic?> GetTopicByIdAsync(int topicId);
        Task<Topic?> AddTopicAsync(int userId, string title, string content, string author, string categories, string tags);
        Task<bool> DeleteTopicAsync(int topicId);
        Task<Topic?> UpdateTopicStatusAsync(int topicId, bool newStatus);
    }
} 