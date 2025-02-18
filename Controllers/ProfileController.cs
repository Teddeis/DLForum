using System.Xml.Linq;
using Profile;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Supabase.Gotrue;
using DLForum.Models.Topic;

namespace DLForum.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(ProfileUserService userService, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<IActionResult> Profile()
        {
            // Получение текущего пользователя из сессии
            var currentUser = HttpContext.Session.GetInt32("ID");

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Асинхронно получаем данные о темах и комментариях пользователя
            var topics = await _userService.GetTopicsByAuthorAsync((int)currentUser);
            var comments = await _userService.GetCommentsByAuthorAsync((int)currentUser);
            var favorites = await _userService.GetFavorite((int)currentUser);

            // Создаем модель для передачи в представление
            var profile = new Profile.Profile
            {
                user = new users { id = (int)currentUser }, // id пользователя
                TopicsList = topics, // Полученные темы
                CommentsList = comments, // Полученные комментарии
                FavoritesList = favorites
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
            var profile = new Profile.Profile
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
        public async Task<IActionResult> UpdateProfile(Profile.ProfileSettingsViewModel model, IFormFile avatar, IFormFile banner)
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

                var supabaseUrl = _configuration["Supabase:Url"];
                var supabaseKey = _configuration["Supabase:Key"];
                var supabase = new Supabase.Client(supabaseUrl, supabaseKey);
                await supabase.InitializeAsync();

                var storage = supabase.Storage;

        // Загрузка аватара в Supabase Storage
        if (avatar != null && avatar.Length > 0)
        {
            string avatarFileName = $"{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
            using (var memoryStream = new MemoryStream())
            {
                await avatar.CopyToAsync(memoryStream);
                byte[] avatarBytes = memoryStream.ToArray();

                var bucket = storage.From("avatars");
                var response = await bucket.Upload(avatarBytes, avatarFileName);

                if (response != null)
                {
                    user.avatar_url = $"{supabaseUrl}/storage/v1/object/public/avatars/{avatarFileName}";
                }
            }
        }

        // Загрузка баннера в Supabase Storage
        if (banner != null && banner.Length > 0)
        {
            string bannerFileName = $"{Guid.NewGuid()}{Path.GetExtension(banner.FileName)}";
            using (var memoryStream = new MemoryStream())
            {
                await banner.CopyToAsync(memoryStream);
                byte[] bannerBytes = memoryStream.ToArray();

                var bucket = storage.From("banners");
                var response = await bucket.Upload(bannerBytes, bannerFileName);

                if (response != null)
                {
                    user.you_background = $"{supabaseUrl}/storage/v1/object/public/banners/{bannerFileName}";
                }
            }
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

                var updatedUser = await _userService.UpdateUserAsync(user);
                if (updatedUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Failed to update user.");
                    return View(model);
                }

                HttpContext.Session.SetString("Background", updatedUser.you_background ?? "");
                HttpContext.Session.SetString("AvatarUrl", updatedUser.avatar_url ?? "");
                HttpContext.Session.SetString("UserName", updatedUser.username ?? "");
                HttpContext.Session.SetString("Gender", updatedUser.gender ?? "");
                HttpContext.Session.SetString("About", updatedUser.about ?? "");

                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error updating profile: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while updating the profile.");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var passwordHasher = new PasswordHasher<users>();
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null || !passwordHasher.VerifyHashedPassword(user, user.password_hash, currentPassword).Equals(PasswordVerificationResult.Success))
            {
                ModelState.AddModelError("", "Неверный текущий пароль.");
                return View("Settings");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают.");
                return View("Settings");
            }

            user.password_hash = passwordHasher.HashPassword(user, newPassword);
            await _userService.UpdateUserAsync(user);
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string newEmail, string password)
        {
            var passwordHasher = new PasswordHasher<users>();
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (!passwordHasher.VerifyHashedPassword(user, user.password_hash, password).Equals(PasswordVerificationResult.Success))
            {
                ModelState.AddModelError("", "Неверный пароль.");
                return View("Settings");
            }

            user.email = newEmail;
            await _userService.UpdateUserAsync(user);
            HttpContext.Session.SetString("Email", newEmail);

            return RedirectToAction("Settings");
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
