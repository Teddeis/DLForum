using Microsoft.AspNetCore.Mvc;

namespace DLForum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileUserService _userService;

        public ProfileController(ProfileUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Users(int id)
        {
            if (_userService == null)
            {
                return NotFound("Ошибка на стороне сервера.");
            }

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return View(user);
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Notification()
        {
            return View();
        }
    }
}
