using System.Diagnostics;
using DLForum.Models;
using Microsoft.AspNetCore.Mvc;
using DLForum.Service; // Добавьте пространство имен, если ваш TopicService находится здесь

namespace DLForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TopicService _topicService;

        public HomeController(ILogger<HomeController> logger, TopicService topicService)
        {
            _logger = logger;
            _topicService = topicService;
        }

        // Метод для отображения главной страницы с пагинацией
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                int pageSize = 13; // Число тем на одной странице

                // Получаем темы и общее количество с пагинацией
                var (topics, totalCount) = await _topicService.GetTopicsByPageAsync(pageNumber, pageSize);

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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
