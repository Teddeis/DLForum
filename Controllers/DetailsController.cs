using DLForum.Models.Topic;
using DLForum.Service;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;

public class DetailsController : Controller
{
    private readonly DetailsService _topicService;
    private readonly CommentService _commentService; // Assuming a CommentService for fetching comments
    private readonly ImageService _imageService;
    private readonly FavoriteService _favoriteService;


    public DetailsController(DetailsService topicService, CommentService commentService, ImageService imageService, FavoriteService favoriteService)
    {
        _topicService = topicService;
        _commentService = commentService;
        _imageService = imageService;
        _favoriteService = favoriteService;
    }

    [HttpGet("/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            // Получаем данные по теме
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }

            // Получаем комментарии и пользователей
            var comments = await _commentService.GetCommentsByTopicIdAsync(id);
            var images = await _imageService.GetImagesByTopicIdAsync(id);

            // Упрощаем данные об изображениях
            var imageUrls = images.Select(img => new { img.ImageUrl }).ToList();

            var userId = HttpContext.Session.GetInt32("ID");
            var isFavorite = false;
            var isLike = false;

            if (userId != null)
            {
                // Проверяем, является ли эта тема избранной для текущего пользователя
                isFavorite = await _favoriteService.IsFavorite(userId.Value, id);
                isLike = await _favoriteService.IsLike(userId.Value, id);
            }

            // Передаем данные через ViewBag
            ViewBag.Comments = comments;
            ViewBag.Images = imageUrls;
            ViewBag.IsFavorite = isFavorite;
            ViewBag.IsLike = isLike;

            // Передаем саму модель (topic) в представление
            return View(topic);
        }
        catch (Exception ex)
        {
            // Логируем ошибку, если возникла
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

            var images = await _imageService.GetImagesByTopicIdAsync(id);

            var imageUrls = images.Select(img => new { img.ImageUrl }).ToList(); // Only keep the URL


            ViewBag.Images = imageUrls; // Send simplified list

            return View(topic); // Отправляем тему в представление
        }
        catch (Exception ex)
        {
            // В случае ошибки выводим сообщение
            ViewBag.ErrorMessage = ex.Message;
            return View("Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(int id_topic, string comment)
    {
        var userId = HttpContext.Session.GetInt32("ID");
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrWhiteSpace(comment))
        {
            TempData["ErrorMessage"] = "Комментарий не может быть пустым.";
            return RedirectToAction("Details", new { id = id_topic });
        }

        try
        {
            Console.WriteLine($"Полученные данные: id_user={userId}, id_topic={id_topic}, comment={comment}");

            await _commentService.AddCommentAsync(userId.Value, id_topic, comment);

            var commentCount = await _favoriteService.GetCommentCount(id_topic);
            await _topicService.UpdateCommentCount(id_topic, commentCount);

            TempData["SuccessMessage"] = "Комментарий успешно добавлен!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ИСКЛЮЧЕНИЕ: {ex.Message}");
            TempData["ErrorMessage"] = "Не удалось добавить комментарий: " + ex.Message;
        }

        return RedirectToAction("Details", new { id = id_topic });
    }

    [HttpPost]
    public async Task<IActionResult> CreateReply(int parentId, int id_topic, string replyText)
    {
        var userId = HttpContext.Session.GetInt32("ID");

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Логирование полученных данных
            Console.WriteLine($"Полученные данные: parentId={parentId}, id_topic={id_topic}, replyText={replyText}");

            if (string.IsNullOrWhiteSpace(replyText))
            {
                Console.WriteLine("ОШИБКА: Ответ пустой!");
                ModelState.AddModelError("Reply", "Ответ не может быть пустым.");
            }
            else
            {
                // Добавляем новый ответ
                await _commentService.AddReplyAsync(userId.Value, parentId, replyText);
                TempData["SuccessMessage"] = "Ответ успешно добавлен!";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ИСКЛЮЧЕНИЕ: {ex.Message}");
            TempData["ErrorMessage"] = "Не удалось добавить ответ: " + ex.Message;
        }

        return RedirectToAction("Details", new { id = id_topic });
    }



    [HttpPost]
    public async Task<IActionResult> ToggleFavorite(int id_topics)
    {
        var userId = HttpContext.Session.GetInt32("ID");

        if (userId == null)
        {
            return Json(new { error = "NotLoggedIn" }); // Вернуть ошибку, если пользователь не авторизован
        }

        var isFavorite = await _favoriteService.IsFavorite(userId.Value, id_topics);

        if (isFavorite)
        {
            await _favoriteService.RemoveFavorite(userId.Value, id_topics);
        }
        else
        {
            await _favoriteService.AddFavorite(userId.Value, id_topics);
        }

        isFavorite = !isFavorite; 
        return Json(new { isFavorite });
    }

    [HttpPost]
    public async Task<IActionResult> ToggleLike(int id_topics)
    {
        var userId = HttpContext.Session.GetInt32("ID");

        if (userId == null)
        {
            return Json(new { error = "NotLoggedIn" }); // Ошибка, если пользователь не авторизован
        }

        var isLike = await _favoriteService.IsLike(userId.Value, id_topics);

        if (isLike)
        {
            await _favoriteService.RemoveLike(userId.Value, id_topics);
        }
        else
        {
            await _favoriteService.AddLike(userId.Value, id_topics);
        }

        // Инвертируем состояние
        isLike = !isLike;

        // Подсчитываем актуальное количество лайков
        var likeCount = await _favoriteService.GetLikeCount(id_topics);

        // Обновляем количество лайков в `topics`
        await _topicService.UpdateLikesCount(id_topics, likeCount);

        return Json(new { isLike, likeCount });
    }

}
