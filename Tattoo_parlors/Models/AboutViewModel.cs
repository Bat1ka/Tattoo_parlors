namespace Tattoo_parlors.Models
{
    public class AboutViewModel
    {
        public IEnumerable<Tattoo.ServicesModel> Services { get; set; }
        public Tattoo_parlors.Models.SalonInfo SalonInfo { get; set; }
    }
}
