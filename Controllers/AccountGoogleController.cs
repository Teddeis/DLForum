using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class AccountGoogleController : Controller
{
    private readonly UserService _userService;

    public AccountGoogleController(UserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    // Старт аутентификации через Google
    [HttpGet]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "AccountGoogle");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    // Обработка ответа от Google
    [HttpGet]
    public async Task<IActionResult> GoogleResponse()
    {
        // Проверка аутентификации
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!authenticateResult.Succeeded)
        {
            Console.WriteLine("Authentication failed.");
            return RedirectToAction("Login", "Account");
        }

        // Получение данных из Claims
        var claims = authenticateResult.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var avatarUrl = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

        if (string.IsNullOrEmpty(email))
        {
            Console.WriteLine("No email found.");
            return RedirectToAction("Login", "Account");
        }

        // Проверка, есть ли пользователь в базе данных
        var user = await _userService.LoginAsync(email, ""); // Не передаем пароль

        if (user == null)
        {
            Console.WriteLine("User not found, creating new user...");
            // Создаем нового пользователя
            user = await _userService.RegisterAsync(email, "GoogleAuth");

            if (user == null)
            {
                Console.WriteLine("Failed to create user.");
                ModelState.AddModelError("", "Не удалось создать аккаунт.");
                return RedirectToAction("Login", "Account");
            }
        }
        else
        {
            Console.WriteLine($"User found: {user.username}"); // Логирование найденного пользователя
        }

        // Добавляем данные пользователя в Claims
        var claimsIdentity = new ClaimsIdentity(new[]
        {
        new Claim(ClaimTypes.Name, user.username ?? name ?? "Без имени"),
        new Claim(ClaimTypes.Email, user.email),
        new Claim("Role", user.role ?? "user"),
        new Claim("Gender", user.gender ?? "Не указан"),
        new Claim("About", user.about ?? ""),
        new Claim("AvatarUrl", user.avatar_url ?? avatarUrl ?? ""),
        new Claim("Background", user.you_background ?? ""),
    }, CookieAuthenticationDefaults.AuthenticationScheme);

        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        // Обновляем аутентификацию с новыми данными
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);


        HttpContext.Session.SetInt32("ID", user.id);
        HttpContext.Session.SetString("Email", user.email);
        HttpContext.Session.SetString("UserName", user.username);
        HttpContext.Session.SetString("Role", user.role);
        HttpContext.Session.SetString("Gender", user.gender);
        HttpContext.Session.SetString("About", user.about);
        HttpContext.Session.SetString("AvatarUrl", user.avatar_url);
        HttpContext.Session.SetString("Background", user.you_background);

        // Печать данных для проверки
        Console.WriteLine($"User signed in: {user.username}");

        // Редирект на главную страницу
        return RedirectToAction("Index", "Home");
    }
}
