using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;
using System.Threading.Tasks;
using static Tattoo_parlors.Models.Tattoo;

namespace Tattoo_parlors.Controllers
{
    public class SketchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SketchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Асинхронная версия метода, позволяющая фильтровать эскизы по tattooistId
        public async Task<IActionResult> Sketches(int tattooistId = 0)
        {
            // Если был передан конкретный id, проверяем, что такой тату-мастер существует
            if (tattooistId != 0 && !await _context.Tattooist.AnyAsync(t => t.id == tattooistId))
            {
                return NotFound();
            }

            // Подготавливаем запрос с включением данных о тату-мастере
            var sketchesQuery = _context.Sketch
                .Include(s => s.Tattooist)
                .AsQueryable();

            if (tattooistId != 0)
            {
                sketchesQuery = sketchesQuery.Where(s => s.tattooistId == tattooistId);
            }

            // Собираем модель представления
            var viewModel = new SketchesViewModel
            {
                Sketches = await sketchesQuery.ToListAsync(),
                Tattooists = await _context.Tattooist.ToListAsync(),
                SelectedTattooistId = tattooistId
            };

            return View(viewModel);
        }

        // Асинхронный метод для отображения деталей эскиза
        public async Task<IActionResult> Details(int id)
        {
            var sketch = await _context.Sketch
                .Include(s => s.Tattooist)
                .FirstOrDefaultAsync(s => s.id == id);

            if (sketch == null)
            {
                return NotFound();
            }

            return View(sketch);
        }
    }
}
