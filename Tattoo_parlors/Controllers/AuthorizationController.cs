using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace Tattoo_parlors.Controllers
{
public class AuthorizationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
       
        public AuthorizationController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("void")]
    public IActionResult Void()
    {
        return View();
    }
    [HttpPost("Void")]
    public async Task<IActionResult> Void(string email, string password)
    {
        // Хэширование введенного пароля для сравнения
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            password = Convert.ToBase64String(hashedBytes);
        }

        // Поиск пользователя с соответствующими данными
        var user = _context.Users.FirstOrDefault(u => u.email == email && u.password == password);

        // После успешного поиска пользователя в базе
        if (user != null)
        {
            // Преобразуем числовую роль в строку: "Admin" для 1, "User" для 0
            var roleStr = user.role == 1 ? "Admin" : "User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.email),
                new Claim("UserName", user.name),
                new Claim(ClaimTypes.Role, roleStr)  // Добавляем claim роли
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Glav", "Home"); // Перенаправление на главную страницу
    }
        else
        {
            // Ошибка аутентификации
            ViewBag.ErrorMessage = "Неверный email или пароль.";
            return View();
        }
    }
    public IActionResult Login(string email, string password)
    {
        // Пример проверки учетных данных
        var user = _context.Users.FirstOrDefault(u => u.email == email && u.password == password);
        if (user != null)
        {
            // Сохраняем данные в сессии
            HttpContext.Session.SetString("UserEmail", user.email);
            HttpContext.Session.SetString("UserName", user.name);

            return RedirectToAction("Glav", "Home");
        }
        else
        {
            ViewBag.ErrorMessage = "Неверные учетные данные.";
            return View();
        }
    }

    public IActionResult Logout()
    {
        // Удаление данных из сессии
        HttpContext.Session.Clear();

        // Удаление куки аутентификации
        Response.Cookies.Delete(".AspNetCore.Cookies");

        // Перенаправление на главную страницу
        return RedirectToAction("Glav", "Home");
    }

    [HttpGet("Registration")]
    public IActionResult Registration()
    {
        return View(); 
    }
        [HttpPost("Registration")]
        public async Task<IActionResult> Registration(UsersModel userModel)
        {
            // Хэширование пароля
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(userModel.password));
                userModel.password = Convert.ToBase64String(hashedBytes);
            }

            // Добавляем пользователя в базу данных
            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();

            // Преобразование числовой роли в строку
            var roleStr = userModel.role == 1 ? "Admin" : "User";

            // Вставляем нужный код здесь: формируем список claims, включая идентификатор пользователя
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userModel.id.ToString()), // добавляем идентификатор пользователя
        new Claim(ClaimTypes.Name, userModel.email),
        new Claim("UserName", userModel.name),
        new Claim(ClaimTypes.Role, roleStr)
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Glav", "Home");
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

    }
}
