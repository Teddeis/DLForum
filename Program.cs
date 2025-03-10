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
        app.UseCors("AllowAll"); // ��������� ��� ������
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

        // ����������� ���������������� ��������
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




        // ����������� ������
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true; // ������ ������ ����� HTTP
            options.Cookie.IsEssential = true; // ���� ����������� ��� ������ ����������
        });


        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login"; // ���� � �������� �����
                options.LogoutPath = "/Account/Logout"; // ���� � �������� ������
            }).AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.CallbackPath = "/signin-google"; // ������ ��� ��������� ������
            });

        // ����������� HttpContextAccessor ��� ������� � �������� ���������
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // ��� ��������� ������������ � ����������
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // ����������� ������ ����� ������������
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();

        // ��������� ���������
        app.MapDefaultControllerRoute();


        app.UseDeveloperExceptionPage(); // Ensure this is enabled in development mode.

        app.Run();
    }
}