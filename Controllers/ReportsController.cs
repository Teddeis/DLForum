using Microsoft.AspNetCore.Mvc;
using DLForum.Service;
using DLForum.Models.Comments;
using Microsoft.AspNetCore.Http;

namespace DLForum.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ProfileUserService _userService;

        public ReportsController(ProfileUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> reports()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "moderator")
            {
                return Forbid();
            }

            var reports = await _userService.GetReportsAsync();
            return View(reports);
        }
    }
} 