using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);



// ��������� ������ �����������
builder.Services.AddMemoryCache();
// ������ https://joker-tattoo.by/


builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8,2))));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� ����� ������
    options.Cookie.HttpOnly = true; // ����������� ������� ����� JavaScript
    options.Cookie.IsEssential = true; // ���� ���������� ��� ������ ����������
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authorization/Login";
        options.AccessDeniedPath = "/Authorization/AccessDenied";
    });

var app = builder.Build();





// �������� ��������� IMemoryCache �� ����������
using (var scope = app.Services.CreateScope())
{
    var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

    // ������� ����
    var keys = new[] { "key1", "key2", "key3" }; // ���� �����
    foreach (var key in keys)
    {
        cache.Remove(key);
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession(); // �������� ��������� ������

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Glav}/{id?}");

app.Run();