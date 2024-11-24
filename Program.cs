var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(new SupabaseClientService(
    builder.Configuration["Supabase:Url"],
    builder.Configuration["Supabase:Key"]
));

// ����������� ���������������� ��������
builder.Services.AddScoped<UserService>();

// ����������� ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // ������ ������ ����� HTTP
    options.Cookie.IsEssential = true; // ���� ����������� ��� ������ ����������
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
app.UseAuthorization();

// ��������� ���������
app.MapDefaultControllerRoute();

app.Run();