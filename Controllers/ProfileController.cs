using System.Xml.Linq;
using DLForum.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Supabase.Gotrue;

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

        public async Task<IActionResult> Profile()
        {
            // Получение текущего пользователя из сессии
            var currentUser = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(currentUser))
            {
                return RedirectToAction("Login", "Account");
            }

            // Асинхронно получаем данные о темах и комментариях пользователя
            var topics = await _userService.GetTopicsByAuthorAsync(currentUser);
            var comments = await _userService.GetCommentsByAuthorAsync(currentUser);

            // Создаем модель для передачи в представление
            var profile = new Profile
            {
                user = new users { email = currentUser }, // Тут только email, если нужна полная информация, надо вызвать GetUserByIdAsync или аналогичный метод
                TopicsList = topics, // Полученные темы
                CommentsList = comments // Полученные комментарии
            };

            return View(profile);
        }



        public async Task<IActionResult> Users(int id)
        {
            // Получаем пользователя по ID
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            // Асинхронно получаем данные о темах и комментариях текущего пользователя
            var topics = await _userService.GetTopicsByIdAsync(user.id);
            var comments = await _userService.GetCommentsByIdAsync(user.id);

            // Создаем модель профиля
            var profile = new Profile
            {
                user = new users
                {
                    id = user.id, // Передаем ID пользователя
                    username = user.username, // Передаем имя пользователя
                    email = user.email, // Передаем email пользователя
                    role = user.role, // Передаем роль пользователя (если есть)
                    avatar_url = user.avatar_url, // Передаем аватар пользователя (если есть)
                    gender = user.gender,
                    you_background = user.you_background,
                    about = user.about
                },
                TopicsList = topics, // Список тем
                CommentsList = comments // Список комментариев
            };

            // Передаем профиль в представление
            return View(profile);
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
