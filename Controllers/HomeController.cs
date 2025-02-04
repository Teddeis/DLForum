using System.Diagnostics;
using DLForum.Models;
using Microsoft.AspNetCore.Mvc;

namespace DLForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TopicService _topicService;
        private readonly CommentService _commentService;

        public HomeController(ILogger<HomeController> logger, TopicService topicService, CommentService commentService)
        {
            _logger = logger;
            _topicService = topicService;
            _commentService = commentService;
        }

        // Метод для отображения главной страницы с пагинацией
        public async Task<IActionResult> Home(int pageNumber = 1)
        {

            try
            {
                int pageSize = 10; // Число тем на одной странице

                // Получаем темы и общее количество с пагинацией
                var (topics, totalCount) = await _topicService.GetTopicsByPageAsync(pageNumber, pageSize);



                // Рассчитываем количество страниц
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Передаем данные в представление
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;


                return PartialView(topics);
            }
            catch (Exception ex)
            {
                // В случае ошибки, можно отобразить сообщение
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> Categories(string category, int pageNumber = 1)
        {
            try
            {
                int pageSize = 10; // Количество тем на странице

                // Получаем темы с фильтрацией по категории и пагинацией
                var (topics, totalCount) = await _topicService.GetTopicsByPageCategoriesAsync(category, pageNumber, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Передаем данные в представление
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.SelectedCategory = category;

                return PartialView(topics); // Загружаем в текущее представление
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        // Метод для отображения главной страницы с пагинацией
        public async Task<IActionResult> Popular(int pageNumber = 1)
        {

            try
            {
                int pageSize = 10; // Число тем на одной странице

                // Получаем темы и общее количество с пагинацией
                var (topics, totalCount) = await _topicService.GetTopicsByPagePopularAsync(pageNumber, pageSize);



                // Рассчитываем количество страниц
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Передаем данные в представление
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;


                return PartialView(topics);
            }
            catch (Exception ex)
            {
                // В случае ошибки, можно отобразить сообщение
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Rules()
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
