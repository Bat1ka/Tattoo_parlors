using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using static Tattoo_parlors.Models.Tattoo;

namespace Tattoo_parlors.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var artists = _context.Tattooist.ToList();
            var viewModel = new AppointmentViewModel
            {
                TattooArtists = artists,
                BookingData = new BookAppointmentViewModel()
            };
            return View(viewModel);
        }
        // AJAX-метод для получения слотов, доступных для записи
        public IActionResult GetAvailableSlots(int tattooArtistId, DateTime date)
        {
            // Получаем шаблоны слотов для выбранного мастера
            var templates = _context.ScheduleTemplate
                            .Where(t => t.tattooArtistId == tattooArtistId)
                            .OrderBy(t => t.slotNumber)
                            .ToList();

            // Получаем номера занятых слотов на указанную дату
            var bookedSlots = _context.Appointment
                              .Where(a => a.tattooArtistId == tattooArtistId && a.date.Date == date.Date)
                              .Select(a => a.slotNumber)
                              .ToList();

            // Фильтруем доступные слоты
            var availableSlots = templates.Where(t => !bookedSlots.Contains(t.slotNumber))
                .Select(t => new {
                    slotNumber = t.slotNumber,
                    startTime = t.startTime.ToString(@"hh\:mm")
                })
                .ToList();

            return Json(availableSlots);
        }

        [HttpPost]
        public IActionResult BookAppointment(BookAppointmentViewModel model)
        {
            // Извлечение идентификатора авторизованного пользователя
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                ModelState.AddModelError(string.Empty, "Ошибка получения идентификатора пользователя.");
                model.TattooArtists = _context.Tattooist.ToList();
                return View("Index", new AppointmentViewModel
                {
                    TattooArtists = model.TattooArtists,
                    BookingData = model
                });
            }

            // Проверка, что дата - не выходной (суббота или воскресенье)
            if (model.Date.DayOfWeek == DayOfWeek.Saturday || model.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                ModelState.AddModelError(string.Empty, "Запись на выходные дни невозможна. Пожалуйста, выберите будний день (Пн-Пт).");
                model.TattooArtists = _context.Tattooist.ToList();
                return View("Index", new AppointmentViewModel
                {
                    TattooArtists = model.TattooArtists,
                    BookingData = model
                });
            }

            // Проверка на наличие уже существующей записи
            bool exists = _context.Appointment.Any(a =>
                                a.tattooArtistId == model.TattooArtistId &&
                                a.date.Date == model.Date.Date &&
                                a.slotNumber == model.SlotNumber);

            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Данный слот уже занят. Выберите другой слот.");
                model.TattooArtists = _context.Tattooist.ToList();
                return View("Index", new AppointmentViewModel
                {
                    TattooArtists = model.TattooArtists,
                    BookingData = model
                });
            }

            // Создаём новую запись, используя полученный userId
            var appointment = new Appointment
            {
                tattooArtistId = model.TattooArtistId,
                date = model.Date,
                slotNumber = model.SlotNumber,
                userId = userId
            };

            _context.Appointment.Add(appointment);

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Не удалось записаться, возможно слот уже занят. Попробуйте еще раз.");
                model.TattooArtists = _context.Tattooist.ToList();
                return View("Index", new AppointmentViewModel
                {
                    TattooArtists = model.TattooArtists,
                    BookingData = model
                });
            }

            return RedirectToAction("Profil", "Prof");
        }



    }
}
