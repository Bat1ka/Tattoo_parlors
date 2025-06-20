using Microsoft.AspNetCore.Mvc;
using Tattoo_parlors.Data;
using Tattoo_parlors.Models;

namespace Tattoo_parlors.ViewComponents
{
    public class SalonInfoViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public SalonInfoViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            var salon = _context.SalonInfo.FirstOrDefault();
            return View(salon);
        }
    }
}
