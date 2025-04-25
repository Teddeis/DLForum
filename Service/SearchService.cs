using Microsoft.Extensions.Caching.Memory;


public class SearchService
{
    private readonly IMemoryCache _cache;
    private readonly Supabase.Client _supabaseClient;

    public SearchService(IMemoryCache cache, Supabase.Client supabaseClient)
    {
        _cache = cache;
        _supabaseClient = supabaseClient;
    }

    public async Task<List<SearchResult>> GetSuggestionsAsync(string query)
    {
        query = query.ToLower();

        // Разбираем запрос на компоненты
        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var tags = terms.Where(t => t.StartsWith("#")).Select(t => t.Substring(1)).ToList();
        var category = terms.FirstOrDefault(t => t.StartsWith("category:"))?.Substring(9)?.ToLower();
        var searchTerms = terms.Where(t => !t.StartsWith("#") && !t.StartsWith("category:")).ToList();

        // Получаем темы из кэша или Supabase
        var topics = await GetCachedTopicsAsync();

        // Фильтруем результаты
        var results = topics.Where(topic =>
        {
            bool matchesSearch = searchTerms.Count == 0 ||
                searchTerms.Any(term => topic.Title.ToLower().Contains(term));

            bool matchesTags = tags.Count == 0 ||
                tags.All(tag => topic.Tags.Any(t => t.ToLower() == tag.ToLower()));

            bool matchesCategory = string.IsNullOrEmpty(category) ||
                topic.Category.ToLower() == category;

            return matchesSearch && matchesTags && matchesCategory;
        }).ToList();

        return results;
    }

    private async Task<List<SearchResult>> GetCachedTopicsAsync()
    {
        const string CacheKey = "AllTopics";

        if (!_cache.TryGetValue(CacheKey, out List<SearchResult> topics))
        {
            // Получаем данные из Supabase
            var response = await _supabaseClient
                .From<Topic>()
                .Select("*")
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            var topicsList = response.Models;

            topics = topicsList.Select(t => new SearchResult
            {
                Title = t.Title,
                Category = t.Categories, // У вас это строка, возможно нужно разделить если там несколько категорий
                Tags = t.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList() ?? new List<string>(),
                Url = $"/topic/{t.Id}",
                ImageUrl = t.ImageUrl,
                Author = t.Author,
                CommentsCount = t.CommentsCount,
                LikesCount = t.LikesCount
            }).ToList();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(CacheKey, topics, cacheOptions);
        }

        return topics;
    }

    // Метод для обновления кэша при создании новой темы
    public async Task AddToIndexAsync(Topic topic)
    {
        const string CacheKey = "AllTopics";

        var searchResult = new SearchResult
        {
            Title = topic.Title,
            Category = topic.Categories,
            Tags = topic.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList() ?? new List<string>(),
            Url = $"/details/{topic.Id}",
            ImageUrl = topic.ImageUrl,
            Author = topic.Author,
            CommentsCount = topic.CommentsCount,
            LikesCount = topic.LikesCount
        };

        var topics = await GetCachedTopicsAsync();
        topics.Add(searchResult);

        _cache.Set(CacheKey, topics, new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(10)));
    }

    public void InvalidateCache()
    {
        const string CacheKey = "AllTopics";
        _cache.Remove(CacheKey);
    }
}