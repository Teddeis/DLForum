using DLForum.Service;
using Microsoft.AspNetCore.Authentication.Cookies;

internal class Program
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors("AllowAll"); // Разрешаем все домены
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

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
        builder.Services.AddScoped<ImageService>();
        builder.Services.AddScoped<ProfileUserService>();
        builder.Services.AddScoped<SearchService>();
        builder.Services.AddScoped<FavoriteService>();
        builder.Services.AddScoped<NotificationService>();

        builder.Services.AddScoped<NotificationCountFilter>();

        builder.Services.AddControllersWithViews(options =>
        {
            options.Filters.Add<NotificationCountFilter>(); 
        });




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
            }).AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google"; // Строка для обратного вызова
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
        app.UseAuthentication();
        app.UseAuthorization();

        // Настройка маршрутов
        app.MapDefaultControllerRoute();


        app.UseDeveloperExceptionPage(); // Ensure this is enabled in development mode.

        app.Run();
    }
}