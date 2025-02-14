using Microsoft.AspNetCore.Mvc;

public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }


    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Не найдено");
        }

        try
        {
            var suggestions = await _searchService.GetSuggestionsAsync(query);

            // Если нет результатов
            if (suggestions == null || !suggestions.Any())
            {
                return NotFound("Не найдено");
            }

            return Ok(suggestions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}


