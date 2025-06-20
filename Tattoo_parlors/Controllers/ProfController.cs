using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using static Tattoo_parlors.Models.Tattoo;

namespace Tattoo_parlors.Controllers
{
    public class ProfController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Profil()
        {
            // Получаем идентификатор текущего пользователя из Claims
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // Загружаем данные пользователя
            var currentUser = _context.Users.FirstOrDefault(u => u.id == currentUserId);

            // Загружаем записи для текущего пользователя
            var userAppointments = _context.Appointment
                .Include(a => a.User)
                .Include(a => a.TattooArtist)
                    .ThenInclude(ta => ta.ScheduleTemplates)
                .Where(a => a.userId == currentUserId)
                .ToList();

            // Загружаем данные о тату-мастерах
            var tattooArtists = _context.Tattooist
                .Include(a => a.ScheduleTemplates)
                .ToList();

            // Составляем модель представления
            var viewModel = new AdminViewModel
            {
                Appointments = userAppointments,
                TattooArtists = tattooArtists,
                CurrentUser = currentUser
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAppointment(int appointmentId)
        {
            if (appointmentId == 0)
            {
                return BadRequest("Неверный ID записи.");
            }

            var appointment = await _context.Appointment.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound("Запись не найдена.");
            }

            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            // Можно сделать Redirect на страницу списка записей или в профиль пользователя
            return RedirectToAction("Profil");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var user = _context.Users.FirstOrDefault(u => u.id == currentUserId);
            if (user == null)
                return NotFound();

            // Обновляем email и номер телефона
            user.email = model.Email;
            user.numer = model.PhoneNumber;

            // Если введён новый пароль, то выполняем его хэширование
            if (!string.IsNullOrEmpty(model.Password))
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Password));
                    user.password = Convert.ToBase64String(hashedBytes);
                }
            }

            _context.Update(user);
            _context.SaveChanges();

            return RedirectToAction("Profil");
        }
        [HttpGet]
        public IActionResult EditProfile()
        {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = _context.Users.FirstOrDefault(u => u.id == currentUserId);
            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                Email = user.email,
                PhoneNumber = user.numer
                // Поля для пароля оставляем пустыми, чтобы пользователь сам вводил новый при необходимости
            };

            return View(model);
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
    }
}
