using System.Diagnostics;
using DLForum.Models;
using Microsoft.AspNetCore.Mvc;
using DLForum.Service; // �������� ������������ ����, ���� ��� TopicService ��������� �����

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

        // ����� ��� ����������� ������� �������� � ����������
        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                int pageSize = 10; // ����� ��� �� ����� ��������

                // �������� ���� � ����� ���������� � ����������
                var (topics, totalCount) = await _topicService.GetTopicsByPageAsync(pageNumber, pageSize);



                // ������������ ���������� �������
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // �������� ������ � �������������
                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return View(topics);
            }
            catch (Exception ex)
            {
                // � ������ ������, ����� ���������� ���������
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
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
