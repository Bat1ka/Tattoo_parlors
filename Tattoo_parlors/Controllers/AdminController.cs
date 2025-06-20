using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;
using static Tattoo_parlors.Models.Tattoo;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Tattoo_parlors.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Отображение формы записи
        public IActionResult Index()
        {
            var artists = _context.Tattooist.Include(a => a.ScheduleTemplates).ToList();
            var appointments = _context.Appointment
                                .Include(a => a.User)
                                .Include(a => a.TattooArtist)
                                .OrderBy(a => a.date)
                                .ToList();

            // Получаем список услуг из базы (предполагается, что DbSet называется Services)
            var services = _context.Services.ToList();
            // Получаем список пользователей
            var users = _context.Users.ToList();

            var salonInfos = _context.SalonInfo.ToList(); // Загружаем информацию о салоне

            var viewModel = new AdminViewModel
            {
                TattooArtists = artists,
                Appointments = appointments,
                CreateSlotsData = new CreateSlotsViewModel { SlotTimes = new List<TimeSpan>() },
                Services = services,
                Users = users,
                SalonInfos = salonInfos
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateSlots([Bind(Prefix = "CreateSlotsData")] CreateSlotsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Удаляем существующие шаблоны для выбранного мастера
                var existingTemplates = _context.ScheduleTemplate.Where(t => t.tattooArtistId == model.TattooArtistId);
                _context.ScheduleTemplate.RemoveRange(existingTemplates);
                _context.SaveChanges();

                // Создаем новые шаблоны слотов
                for (int i = 0; i < model.SlotCount; i++)
                {
                    var template = new ScheduleTemplate
                    {
                        tattooArtistId = model.TattooArtistId,
                        slotNumber = i + 1, // нумерация слотов начинается с 1
                        startTime = model.SlotTimes[i]
                    };
                    _context.ScheduleTemplate.Add(template);
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            // Если модель невалидна, создаем полный объект AdminViewModel и возвращаем представление
            var artists = _context.Tattooist.Include(a => a.ScheduleTemplates).ToList();
            var appointments = _context.Appointment
                                .Include(a => a.User)
                                .Include(a => a.TattooArtist)
                                .OrderBy(a => a.date)
                                .ToList();

            var viewModel = new AdminViewModel
            {
                TattooArtists = artists,
                Appointments = appointments,
                CreateSlotsData = model  // Здесь сохраняем введенные данные из формы
            };

            return View("Index", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAppointment(int appointmentId)
        {
            // Поиск записи по идентификатору
            var appointment = _context.Appointment.Find(appointmentId);
            if (appointment != null)
            {
                _context.Appointment.Remove(appointment);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadTattooistImage(int artistId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Выберите файл для загрузки.";
                return RedirectToAction("Index");
            }

            var extension = Path.GetExtension(imageFile.FileName);
            if (!string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Допускается загрузка только файлов в формате JPG.";
                return RedirectToAction("Index");
            }

            // Определяем путь для сохранения файла
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tattooists");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Для простоты сохраняем файл с именем <artistId>.jpg (при повторной загрузке изображение перезапишется)
            var fileName = $"{artistId}.jpg";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            TempData["Success"] = "Изображение успешно загружено.";
            return RedirectToAction("Index", new { activeTab = "artistsTab" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTattooist(string name, string biography)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["Error"] = "Имя тату-мастера не может быть пустым.";
                return RedirectToAction("Index");
            }

            var newTattooist = new TattooistModel
            {
                name = name,
                biography = biography  // сохраняем биографию
            };

            _context.Tattooist.Add(newTattooist);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Тату-мастер успешно добавлен.";
            return RedirectToAction("Index", new { activeTab = "artistsTab" });
        }

        [HttpPost]
        public IActionResult DeleteSlot(int slotTemplateId)
        {
            var slotTemplate = _context.ScheduleTemplate.Find(slotTemplateId);
            if (slotTemplate != null)
            {
                _context.ScheduleTemplate.Remove(slotTemplate);
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { activeTab = "artistsTab" });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTattooist(int tattooistId)
        {
            // Поиск тату-мастера по id
            var tattooist = await _context.Tattooist.FindAsync(tattooistId);
            if (tattooist == null)
            {
                TempData["Error"] = "Тату-мастер не найден.";
                return RedirectToAction("Index");
            }

            // Удаляем связанные шаблоны слотов для выбранного мастера
            var templates = _context.ScheduleTemplate.Where(t => t.tattooArtistId == tattooistId);
            _context.ScheduleTemplate.RemoveRange(templates);

            // Получаем связанные эскизы для выбранного мастера (приводим к списку, чтобы потом иметь возможность работать с данными вне контекста)
            var sketchesList = await _context.Sketch
                .Where(s => s.tattooistId == tattooistId)
                .ToListAsync();

            // Удаляем связанные эскизы из базы
            _context.Sketch.RemoveRange(sketchesList);

            // Удаляем тату-мастера из базы
            _context.Tattooist.Remove(tattooist);

            // Сохраняем изменения в базе данных
            await _context.SaveChangesAsync();

            // Удаляем файл изображения тату-мастера, если он существует
            var tattooistUploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "tattooists");
            var tattooistFilePath = Path.Combine(tattooistUploadsFolder, $"{tattooistId}.jpg");
            if (System.IO.File.Exists(tattooistFilePath))
            {
                System.IO.File.Delete(tattooistFilePath);
            }

            // Удаляем файлы эскизов тату-мастера
            foreach (var sketch in sketchesList)
            {
                // sketch.imagePath хранит относительный путь вида "/images/sketches/filename.jpg"
                var sketchRelativePath = sketch.imagePath.TrimStart('/');
                var sketchFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", sketchRelativePath);
                if (System.IO.File.Exists(sketchFilePath))
                {
                    try
                    {
                        System.IO.File.Delete(sketchFilePath);
                    }
                    catch (Exception ex)
                    {
                        // Здесь можно добавить логирование ошибки удаления файла
                        // Например: _logger.LogError(ex, "Ошибка при удалении файла {FilePath}", sketchFilePath);
                    }
                }
            }

            TempData["Success"] = "Тату-мастер успешно удален.";
            return RedirectToAction("Index", new { activeTab = "artistsTab" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSketch(IFormFile imageFile, int tattooistId, string description)
        {
            // Проверяем, заполнено ли поле description
            if (string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Ошибка: поле описание не заполнено";
                return RedirectToAction("Index", new { activeTab = "artistsTab" });
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Файл не выбран";
                return RedirectToAction("Index", new { activeTab = "artistsTab" });
            }

            // Проверка расширения файла (при желании можно добавить проверку MIME-типа и размера)
            if (Path.GetExtension(imageFile.FileName)?.ToLower() != ".jpg")
            {
                TempData["Error"] = "Поддерживаются только файлы формата JPG";
                return RedirectToAction("Index", new { activeTab = "artistsTab" });
            }

            // Формирование уникального имени файла
            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "sketches");

            // Создаём директорию, если не существует
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Сохранение файла
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Создаем запись в БД
            var sketch = new SketchModel
            {
                tattooistId = tattooistId,
                imagePath = $"/images/sketches/{uniqueFileName}",
                description = description
            };

            _context.Sketch.Add(sketch);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Эскиз загружен успешно";
            return RedirectToAction("Index", new { activeTab = "artistsTab" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddService(string name, string price, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(price) || string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Все поля должны быть заполнены.";
                return RedirectToAction("Index");
            }

            var newService = new ServicesModel
            {
                name = name,
                price = price,
                description = description
            };

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Услуга успешно добавлена.";
            return RedirectToAction("Index", new { activeTab = "servicesTab" });

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                TempData["Error"] = "Услуга не найдена.";
                return RedirectToAction("Index");
            }
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Услуга успешно удалена.";
            return RedirectToAction("Index", new { activeTab = "servicesTab" });
        }
        public async Task<IActionResult> EditService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                TempData["Error"] = "Услуга не найдена.";
                return RedirectToAction("Index");
            }
            return View(service);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditService(ServicesModel model)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Update(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Услуга успешно обновлена.";
                return RedirectToAction("Index", new { activeTab = "servicesTab" });
            }
            return View(model);
        }
        // Пользователи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserRole(int userId, int role)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound("Пользователь не найден");
            }

            user.role = role;
            _context.SaveChanges();

            return RedirectToAction("Index", new { activeTab = "userTab" });
        }

        // Удаление пользователя и связанных с ним данных
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteUser(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return HttpNotFound("Пользователь не найден");
            }

            // Удаляем сообщения, где пользователь является отправителем или получателем
            var messages = _context.Message.Where(m => m.senderId == userId || m.receiverId == userId);
            _context.Message.RemoveRange(messages);

            // Удаляем записи о консультациях, связанные с пользователем
            var appointments = _context.Appointment.Where(a => a.userId == userId);
            _context.Appointment.RemoveRange(appointments);

            // Удаляем самого пользователя
            _context.Users.Remove(user);

            _context.SaveChanges();

            return RedirectToAction("Index", new { activeTab = "userTab" });
        }

        private ActionResult HttpNotFound(string v)
        {
            throw new NotImplementedException();
        }
        //Информаци о салоне 
        
        [HttpGet]
        public IActionResult CreateSalonInfo()
        {
            return View(new SalonInfo());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSalonInfo(SalonInfo salonInfo)
        {
            if (ModelState.IsValid)
            {
                _context.SalonInfo.Add(salonInfo);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(salonInfo);
        }

        // Обновление записи (inline редактирование)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSalonInfo([FromBody] SalonInfo salonInfo)
        {
            if (salonInfo == null)
            {
                return Json(new { success = false, error = "Неверные данные" });
            }
            if (ModelState.IsValid)
            {
                _context.SalonInfo.Update(salonInfo);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, error = "Проверьте введенные данные" });
        }

        // Удаление записи
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSalonInfo([FromBody] int id)
        {
            var salonInfo = _context.SalonInfo.Find(id);
            if (salonInfo == null)
            {
                return Json(new { success = false, error = "Запись не найдена" });
            }
            _context.SalonInfo.Remove(salonInfo);
            _context.SaveChanges();
            return Json(new { success = true });
        }
    }
}
