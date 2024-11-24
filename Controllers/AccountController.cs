﻿using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Register(string username, string email, string password)
    {
        var user = await _authService.RegisterAsync(username, email, password);

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
            HttpContext.Session.SetString("Email", user.email);
            HttpContext.Session.SetString("UserName", user.username);

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
}