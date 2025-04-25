using Microsoft.AspNetCore.Mvc;
using DLForum.Service;
using Supabase;

public class SearchController : Controller
{
    private readonly Client _client;

    public SearchController(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Json(new List<object>());
        }

        try
        {
            // Получаем только одобренные темы, содержащие поисковый запрос
            var response = await _client.From<Topic>()
                .Where(t => t.Status == "true")
                .Get();

            var topics = response.Models;

            var results = topics.Where(t => 
                (t.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.Categories?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (t.Tags?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)
            )
            .Select(t => new
            {
                title = t.Title,
                category = t.Categories,
                tags = t.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(tag => tag.Trim()).ToList(),
                url = $"/details/{t.Id}",
                imageUrl = t.ImageUrl,
                author = t.Author,
                commentsCount = t.CommentsCount,
                likesCount = t.LikesCount
            })
            .Take(10)
            .ToList();

            return Json(results);
        }
        catch (Exception ex)
        {
            return Json(new List<object>());
        }
    }
}