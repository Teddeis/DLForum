using Microsoft.AspNetCore.Authentication.Cookies;

namespace DLForum.Service
{
    public class ConfigureServices
    {
        public void ConfigureService(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // Путь к странице входа
                    options.LogoutPath = "/Account/Logout"; // Путь к действию выхода
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Путь к странице отказа в доступе
                });
        }
    }
}
