using DLForum.Service;
using Microsoft.AspNetCore.Mvc;

public class TopicController : Controller
{
    private readonly TopicService _supabaseService;
    private readonly CommentService _commentService;
    private readonly NotificationService _notificationService;


    public TopicController(TopicService supabaseService, CommentService commentService, NotificationService notificationService)
    {
        _supabaseService = supabaseService;
        _commentService = commentService;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> topiclist(int pageNumber = 1)
    {
        try
        {
            int pageSize = 10;  

            var (topics, totalCount) = await _supabaseService.GetTopicsByPageAsyncAdmin(1, pageSize);

            return View(topics);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View(new { error = ex.Message });
        }
    }

    [HttpGet ("create-topic")]
    public IActionResult topic()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateTopic(string title, string content, string categories, string tags)
    {
        var role = HttpContext.Session.GetString("Role");
        if (role == "banned")
        {
            return Forbid("Забаненные пользователи не могут создавать темы.");
        }
        var userId = HttpContext.Session.GetInt32("ID");
        var author = HttpContext.Session.GetString("UserName");


        if (userId == null)
        {
            return RedirectToAction("login", "account"); 
        }

        await _supabaseService.AddTopicAsync(userId.Value, title, content, author.ToString(), categories, tags);

        TempData["SuccessMessage"] = "Тема отправлена на модерацию";
        return RedirectToAction("index", "home");
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
            return RedirectToAction("topiclist", new { id = topicId });
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
    public async Task<IActionResult> UpdateStatusToNone(int topicId, int userId,string message)
    {
        try
        {
            // Обновляем статус темы на false
            var updatedTopic = await _supabaseService.DeleteTopicStatusAsync(topicId, false);

            var messages = await _notificationService.AddNotificationAsync(userId, message, "System", false,DateTime.Now,"Системное");

            // Перенаправляем на страницу темы после отмены статуса
            return RedirectToAction("topiclist", new { id = topicId });
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Полное удаление темы владельцем:
    [HttpPost]
    public async Task<IActionResult> Delete(int topicId)
    {
        try
        {
            var updatedTopic = await _supabaseService.DeleteTopicAsync(topicId);

            if (updatedTopic == null)
            {
                ViewBag.ErrorMessage = "Тема не найдена!";
                return View("Error");
            }

            return RedirectToAction("index", "home");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }
}
