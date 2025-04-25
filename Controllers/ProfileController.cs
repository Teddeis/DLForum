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
        private readonly NotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public ProfileController(ProfileUserService userService,NotificationService notificationService, IConfiguration configuration)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet ("profile")]
        public async Task<IActionResult> profile()
        {
            var currentUser = HttpContext.Session.GetInt32("ID");

            if (currentUser == null)
            {
                return RedirectToAction("login", "account");
            }

            var topics = await _userService.GetTopicsByAuthorAsync((int)currentUser);
            var comments = await _userService.GetCommentsByAuthorAsync((int)currentUser);
            var favorites = await _userService.GetFavorite((int)currentUser);

            var profile = new Profile.Profile
            {
                user = new users { id = (int)currentUser },
                TopicsList = topics,
                CommentsList = comments,
                FavoritesList = favorites
            };

            return View(profile);
        }


        [HttpGet("profile/{id}")]
        public async Task<IActionResult> users(int id)
        {
            // Получаем пользователя по ID
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("Пользователь не найден.");
            }

            var topics = await _userService.GetTopicsByIdAsync(user.id);
            var comments = await _userService.GetCommentsByIdAsync(user.id);

            var profile = new Profile.Profile
            {
                user = new users
                {
                    id = user.id,
                    username = user.username,
                    email = user.email,
                    role = user.role,
                    avatar_url = user.avatar_url,
                    gender = user.gender,
                    you_background = user.you_background,
                    about = user.about
                },
                TopicsList = topics,
                CommentsList = comments
            };

            return View(profile);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(ProfileSettingsViewModel model, IFormFile avatar, IFormFile banner)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("ID");
                if (userId == null)
                {
                    return RedirectToAction("login", "account");
                }

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user == null)
                {
                    return RedirectToAction("login", "account");
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

                return RedirectToAction("settings");
            }
            catch (Exception ex)
            {
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
                return RedirectToAction("login", "account");
            }

            var user = await _userService.GetUserByIdAsync(userId.Value);
            if (user == null || !passwordHasher.VerifyHashedPassword(user, user.password_hash, currentPassword).Equals(PasswordVerificationResult.Success))
            {
                ModelState.AddModelError("", "Неверный текущий пароль.");
                return View("settings");
            }

            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают.");
                return View("settings");
            }

            user.password_hash = passwordHasher.HashPassword(user, newPassword);
            await _userService.UpdateUserAsync(user);
            return RedirectToAction("settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string newEmail, string password)
        {
            var passwordHasher = new PasswordHasher<users>();
            var userId = HttpContext.Session.GetInt32("ID");
            if (!userId.HasValue)
            {
                return RedirectToAction("login", "account");
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

            return RedirectToAction("settings");
        }

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet("notification")]
        public async Task<IActionResult> notification()
        {
            int? currentUserId = HttpContext.Session.GetInt32("ID");

            if (currentUserId == null)
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(currentUserId.Value);

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(notificationId);
            if (notification != null)
            {
                notification.read = true;
                await _notificationService.UpdateNotificationAsync(notification);
            }

            return RedirectToAction("notification");
        }
    }
}
