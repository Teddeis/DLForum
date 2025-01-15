using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserService _authService;

    // Единый конструктор
    public AccountController(UserService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = await _authService.RegisterAsync(email, password);

        if (user == null)
        {
            ModelState.AddModelError("", "Пользователь с таким email уже существует.");
            return View();
        }

        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        // Проверка входных данных
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email и пароль обязательны.");
            return View();
        }

        try
        {
            // Вызов вашего сервиса авторизации
            var user = await _authService.LoginAsync(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный email или пароль.");
                return View();
            }

            // Сохранение данных пользователя в сессию
            HttpContext.Session.SetInt32("ID", user.id);
            HttpContext.Session.SetString("Email", user.email);
            HttpContext.Session.SetString("UserName", user.username);
            HttpContext.Session.SetString("Role", user.role);
            HttpContext.Session.SetString("Gender", user.gender);
            HttpContext.Session.SetString("About", user.about);
            HttpContext.Session.SetString("AvatarUrl", user.avatar_url);
            HttpContext.Session.SetString("Background", user.you_background);

            // Редирект на главную страницу
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            ModelState.AddModelError("", "Произошла ошибка. Попробуйте позже.");
            return View();
        }
    }

    // Метод для выхода из аккаунта
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Очистка данных сессии
        HttpContext.Session.Clear();

        return RedirectToAction("Index", "Home");
    }
}
