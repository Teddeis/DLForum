using DLForum.Service;
using Microsoft.AspNetCore.Mvc;
using Supabase.Interfaces;
using System.Threading.Tasks;

public class DetailsController : Controller
{
    private readonly DetailsService _topicService;
    private readonly CommentService _commentService; // Assuming a CommentService for fetching comments

    public DetailsController(DetailsService topicService, CommentService commentService)
    {
        _topicService = topicService;
        _commentService = commentService;
    }

    // Метод для отображения подробной информации о теме
    [HttpGet("/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // Получаем тему по ID
            var topic = await _topicService.GetTopicByIdAsync(id);

            if (topic == null)
            {
                return NotFound(); // Возвращаем ошибку, если тема не найдена
            }

            // Получаем комментарии для этой темы
            var comments = await _commentService.GetCommentsByTopicIdAsync(id);

            // Передаем данные в представление
            ViewBag.Comments = comments;

            return View(topic); // Отправляем тему в представление
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Метод для отображения подробной информации о теме для админа
    [HttpGet("/DetailsAdmin/{id}")]
    public async Task<IActionResult> DetailsAdmin(int id)
    {
        try
        {
            // Получаем тему по ID
            var topic = await _topicService.GetTopicByIdAsync(id);

            if (topic == null)
            {
                return NotFound(); // Возвращаем ошибку, если тема не найдена
            }

            // Получаем комментарии для этой темы
            var comments = await _commentService.GetCommentsByTopicIdAsync(id);

            // Передаем данные в представление
            ViewBag.Comments = comments;

            return View(topic); // Отправляем тему в представление
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    // Метод для добавления комментария
    [HttpPost]
    public async Task<IActionResult> CreateComment(string comment, int id_topic)
    {
        var userId = HttpContext.Session.GetInt32("ID");
        var userName = HttpContext.Session.GetString("UserName");
        var avatar = HttpContext.Session.GetString("AvatarUrl");


        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                ModelState.AddModelError("Comment", "Комментарий не может быть пустым.");
            }
            else
            {
                await _commentService.AddCommentAsync(userId.Value, id_topic, userName, avatar, comment);
                TempData["SuccessMessage"] = "Комментарий успешно добавлен!";
            }

        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Не удалось добавить комментарий: " + ex.Message;
        }

        return RedirectToAction("Details", new { id = id_topic });
    }
}
