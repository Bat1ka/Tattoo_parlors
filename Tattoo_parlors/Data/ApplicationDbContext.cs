using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tattoo_parlors.Models;
using static Tattoo_parlors.Models.Tattoo;

namespace Tattoo_parlors.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<UsersModel> Users { get; set; }
        public DbSet<TattooistModel> Tattooist { get; set; }
        public DbSet<MessageModel> Message { get; set; }
        public DbSet<ScheduleTemplate> ScheduleTemplate { get; set; }
        public DbSet<Appointment> Appointment { get; set; }
        public DbSet<SketchModel> Sketch { get; set; }
        public DbSet<ServicesModel> Services{ get; set; }
        public DbSet<SalonInfo> SalonInfo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Создаем уникальный индекс на комбинацию TattooArtistId, Date и SlotNumber для предотвращения двойного бронирования.
            modelBuilder.Entity<Appointment>()
                .HasIndex(a => new { a.tattooArtistId, a.date, a.slotNumber })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
