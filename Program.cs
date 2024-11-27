using DLForum.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddSingleton(new SupabaseClientService(
            builder.Configuration["Supabase:Url"],
            builder.Configuration["Supabase:Key"]
        ));

        // Регистрация пользовательских сервисов
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<TopicService>();
        builder.Services.AddScoped<CommentService>();
        builder.Services.AddScoped<DetailsService>();



        // Подключение сессий
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true; // Доступ только через HTTP
            options.Cookie.IsEssential = true; // Куки обязательны для работы приложения
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // Путь к странице входа
                options.LogoutPath = "/Account/Logout"; // Путь к действию выхода
            });

        // Регистрация HttpContextAccessor для доступа к текущему контексту
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Для повышения безопасности в продакшене
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Подключение сессий перед авторизацией
        app.UseSession();
        app.UseAuthorization();

        // Настройка маршрутов
        app.MapDefaultControllerRoute();

        app.Run();
    }
}