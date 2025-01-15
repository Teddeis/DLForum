using DLForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DLForum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileUserService _userService;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(ProfileUserService userService, IWebHostEnvironment environment)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public IActionResult Profile()
        {
            return View();
        }

        public async Task<IActionResult> Users(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileSettingsViewModel model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("ID");
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Если есть аватар, сохраняем его как Data URL в базу данных
                if (!string.IsNullOrEmpty(model.avatar_url))
                {
                    user.avatar_url = model.avatar_url; 
                }
                if (!string.IsNullOrEmpty(model.you_background))
                {
                    user.you_background = model.you_background; 
                }


                if (!string.IsNullOrEmpty(model.username))
                {
                    user.username = model.username;
                }
                if (!string.IsNullOrEmpty(model.gender))
                {
                    user.gender = model.gender;
                }
                if (!string.IsNullOrEmpty(model.about))
                {
                    user.about = model.about;
                }

                // Update the user in the database
                var updatedUser = await _userService.UpdateUserAsync(user);

                if (updatedUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user.");
                    return View(model);
                }

                // Update session data
                HttpContext.Session.SetString("Background", updatedUser.you_background ?? "");
                HttpContext.Session.SetString("AvatarUrl", updatedUser.avatar_url ?? "");
                HttpContext.Session.SetString("UserName", updatedUser.username ?? "");
                HttpContext.Session.SetString("Gender", updatedUser.gender ?? "");
                HttpContext.Session.SetString("About", updatedUser.about ?? "");

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating profile: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
                return View(model);
            }
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
