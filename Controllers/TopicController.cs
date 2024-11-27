using DLForum.Service;
using Microsoft.AspNetCore.Mvc;

public class TopicController : Controller
{
    private readonly TopicService _supabaseService;
    private readonly CommentService _commentService;


    public TopicController(TopicService supabaseService, CommentService commentService)
    {
        _supabaseService = supabaseService;
        _commentService = commentService;
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
    public async Task<IActionResult> CreateTopic(string title, string content,string author)
    {
        var userId = HttpContext.Session.GetInt32("ID");
        author = HttpContext.Session.GetString("UserName");


        if (userId == null)
        {
            return RedirectToAction("Login", "Account"); // Перенаправление на страницу входа
        }

        // Добавляем новую тему в Supabase
        await _supabaseService.AddTopicAsync(userId.Value, title, content, author);

        return RedirectToAction("Index", "Home"); // Перенаправляем на главную страницу после добавления
    }

    // Метод для обновления статуса темы на true (Принять тему)
    [HttpPost]
    public async Task<IActionResult> UpdateStatusToTrue(int topicId)
    {
        try
        {
            // Обновляем статус темы на true
            var updatedTopic = await _supabaseService.UpdateTopicStatusAsync(topicId, true);

            if (updatedTopic == null)
            {
                // Если тема не была обновлена (не найдена), возвращаем ошибку
                ViewBag.ErrorMessage = "Тема не найдена!";
                return View("Error");
            }

            // Перенаправляем на страницу темы после обновления статуса
            return RedirectToAction("Topic", new { id = topicId });
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Метод для отмены статуса темы (Отменить тему)
    [HttpPost]
    public async Task<IActionResult> UpdateStatusToNone(int topicId)
    {
        try
        {
            // Обновляем статус темы на false
            var updatedTopic = await _supabaseService.DeleteTopicStatusAsync(topicId, false);

            if (updatedTopic == null)
            {
                // Если тема не была обновлена (не найдена), возвращаем ошибку
                ViewBag.ErrorMessage = "Тема не найдена!";
                return View("Error");
            }

            // Перенаправляем на страницу темы после отмены статуса
            return RedirectToAction("TopicList", new { id = topicId });
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // В контроллере:
    [HttpPost]
    public async Task<IActionResult> Delete(int topicId)
    {
        try
        {
            var updatedTopic = await _supabaseService.Delete(topicId);

            if (updatedTopic == null)
            {
                ViewBag.ErrorMessage = "Тема не найдена!";
                return View("Error");
            }

            // Исправленный редирект
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }
}
