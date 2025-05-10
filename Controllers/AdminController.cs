using Microsoft.AspNetCore.Mvc;
using DLForum.Service;
using DLForum.Models.Comments;
using DLForum.Models.Topic;
using Profile;

namespace DLForum.Controllers
{
    public class AdminController : Controller
    {
        private readonly ProfileUserService _userService;
        private readonly NotificationService _notificationService;

        public AdminController(ProfileUserService userService, NotificationService notificationService)
        {
            _userService = userService;
            _notificationService = notificationService;
        }

        [HttpGet("/admin-panel")]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "admin" && role != "moderator")
            {
                return Forbid();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(int userId)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }

            user.role = "banned";
            await _userService.UpdateUserAsync(user);

            // ���������� ����������� ������������
            await _notificationService.CreateNotificationAsync(
                userId,
                "��� ������� ��� ������������ �����������",
                "System",
                "���������"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin")
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound("������������ �� ������");
            }

            user.role = "user";
            await _userService.UpdateUserAsync(user);

            // ���������� ����������� ������������
            await _notificationService.CreateNotificationAsync(
                userId,
                "��� ������� ��� ������������� ���������������",
                "System",
                "���������"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            var comment = await _userService.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return NotFound("����������� �� ������");
            }

            await _userService.DeleteCommentAsync(commentId);

            // ���������� ����������� ������ �����������
            await _notificationService.CreateNotificationAsync(
                comment.id_users,
                "��� ����������� ��� ������ �����������",
                "System",
                "���������"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTopic(int topicId)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            var topic = await _userService.GetTopicByIdAsync(topicId);
            if (topic == null)
            {
                return NotFound("���� �� �������");
            }

            await _userService.DeleteTopicAsync(topicId);

            // ���������� ����������� ������ ����
            await _notificationService.CreateNotificationAsync(
                topic.UserId,
                "���� ���� ���� ������� �����������",
                "System",
                "���������"
            );

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ResolveReport(int reportId)
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            await _userService.ResolveReportAsync(reportId);
            return RedirectToAction("Reports");
        }

        [HttpGet("/reports")]
        public async Task<IActionResult> Reports()
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            var reports = await _userService.GetReportsAsync();
            return View(reports);
        }

        [HttpGet("/user-list")]
        public async Task<IActionResult> userslist()
        {
            var currentUserRole = HttpContext.Session.GetString("Role");
            if (currentUserRole != "admin" && currentUserRole != "moderator")
            {
                return Forbid();
            }

            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }
    }
}