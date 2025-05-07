using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DLForum.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Supabase;
using Supabase.Postgrest;

namespace DLForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SupabaseClientService _supabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TopicService _topicService;

        public HomeController(
            ILogger<HomeController> logger,
            SupabaseClientService supabase,
            IHttpContextAccessor httpContextAccessor,
            TopicService topicService)
        {
            _logger = logger;
            _supabase = supabase;
            _httpContextAccessor = httpContextAccessor;
            _topicService = topicService;
        }

        [Route("home")]
        public async Task<IActionResult> Home(int pageNumber = 1, string sortOrder = "newest")
        {
            try
            {
                int pageSize = 10;
                var (topics, totalCount) = await _topicService.GetTopicsByPageAsync(pageNumber, pageSize, sortOrder);
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.SortOrder = sortOrder;

                return PartialView(topics);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> Index()
        {
            var response = await _supabase.Client
                .From<Topic>()
                .Order(x => x.CreatedAt, Constants.Ordering.Descending)
                .Get();

            return View(response.Models);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Topic topic)
        {
            if (ModelState.IsValid)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userId, out int userIdInt))
                {
                    topic.UserId = userIdInt;
                    topic.CreatedAt = DateTime.UtcNow;

                    await _supabase.Client
                        .From<Topic>()
                        .Insert(topic);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(topic);
        }

        public IActionResult Privacy()
        {
            return PartialView();
        }

        [Route("privacy")]
        public IActionResult FullPrivacy()
        {
            return View("Privacy");
        }

        public IActionResult Rules()
        {
            return PartialView();
        }

        [Route("rules")]
        public IActionResult FullRules()
        {
            return View("Rules");
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreTopics(int pageNumber, string category = null, string view = "home")
        {
            try
            {
                int pageSize = 10;
                var (topics, totalCount) = view switch
                {
                    "categories" => await _topicService.GetTopicsByPageCategoriesAsync(category, pageNumber, pageSize),
                    "popular" => await _topicService.GetTopicsByPagePopularAsync(pageNumber, pageSize),
                    _ => await _topicService.GetTopicsByPageAsync(pageNumber, pageSize)
                };

                var topicsData = topics.Select(t => new
                {
                    id = t.Id,
                    title = t.Title,
                    author = t.Author,
                    imageUrl = t.ImageUrl,
                    tags = t.Tags,
                    categories = t.Categories,
                    likesCount = t.LikesCount,
                    commentsCount = t.CommentsCount,
                    createdAt = t.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                });

                return Json(new
                {
                    topics = topicsData,
                    hasMorePages = pageNumber * pageSize < totalCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> categories(string category, int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;

                var (topics, totalCount) = await _topicService.GetTopicsByPageCategoriesAsync(category, pageNumber, pageSize);

                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;
                ViewBag.SelectedCategory = category;

                return PartialView(topics);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public async Task<IActionResult> popular(int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;
                var (topics, totalCount) = await _topicService.GetTopicsByPagePopularAsync(pageNumber, pageSize);
                int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = totalPages;

                return PartialView(topics);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}