using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly UserService _authService;

    public AccountController(UserService authService)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    }

    [HttpGet]
    public IActionResult register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> register(string email, string password)
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
    public async Task<IActionResult> login(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Email и пароль обязательны.");
            return View();
        }

        try
        {
            var user = await _authService.LoginAsync(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Неверный email или пароль.");
                return View();
            }
            HttpContext.Session.SetInt32("ID", user.id);
            HttpContext.Session.SetString("Email", user.email);
            HttpContext.Session.SetString("UserName", user.username);
            HttpContext.Session.SetString("Role", user.role);
            HttpContext.Session.SetString("Gender", user.gender);
            HttpContext.Session.SetString("About", user.about);
            HttpContext.Session.SetString("AvatarUrl", user.avatar_url);
            HttpContext.Session.SetString("Background", user.you_background);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Произошла ошибка. Попробуйте позже.");
            return View();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
