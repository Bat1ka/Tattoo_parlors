using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;
using static Tattoo_parlors.Models.Tattoo;

namespace Tattoo_parlors.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Salon", "latest.jpg");

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
            }

            return RedirectToAction("O_nas");
        }

        public IActionResult Glav()
        {
            // Получи список мастеров (например, из базы данных или другого источника)
            var tattooists = _context.Tattooist;
            var saloninfo = _context.SalonInfo.FirstOrDefault();

            // Создай экземпляр view model
            var viewModel = new MainPageViewModel
            {
                Tattooists = tattooists,
                SalonInfos = saloninfo,
                
            };

            return View(viewModel);

            
        }

        public IActionResult O_nas()
        {
            // Получение списка услуг из базы данных
            var services = _context.Services.ToList();

            // Получение информации о салоне из базы данных
            // Здесь предполагается, что в таблице SalonInfo хранится одна запись, либо вы выбираете нужную
            var salonInfo = _context.SalonInfo.FirstOrDefault();

            // Создание составной модели
            var viewModel = new AboutViewModel
            {
                Services = services,
                SalonInfo = salonInfo
            };

            return View(viewModel);
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
