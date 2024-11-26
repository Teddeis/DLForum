using Microsoft.AspNetCore.Mvc;

public class TopicController : Controller
{
    private readonly TopicService _supabaseService;

    public TopicController(TopicService supabaseService)
    {
        _supabaseService = supabaseService;
    }

    // Метод для отображения всех тем
    public async Task<IActionResult> TopicList(int pageNumber = 1)
    {
        try
        {
            int pageSize = 10; // Число тем на одной странице

            // Получаем темы и общее количество с пагинацией
            var (topics, totalCount) = await _supabaseService.GetTopicsByPageAsyncAdmin(pageNumber, pageSize);

            // Рассчитываем количество страниц
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Передаем данные в представление
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(topics);
        }
        catch (Exception ex)
        {
            // В случае ошибки, можно отобразить сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Остальные методы контроллера
    public IActionResult Topic()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic(string title, string content)
    {
        var userId = HttpContext.Session.GetInt32("ID");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Перенаправление на страницу входа
        }

        // Добавляем новую тему в Supabase
        await _supabaseService.AddTopicAsync(userId.Value, title, content);

        return RedirectToAction("Index", "Home"); // Перенаправляем на главную страницу после добавления
    }
}
