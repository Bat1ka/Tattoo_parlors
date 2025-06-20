using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tattoo_parlors.Models
{
    public class Tattoo
    {
        public class TattooistModel
        {
            public int id { get; set; }
            public string name { get; set; }
            public string biography { get; set; }
            public List<ScheduleTemplate> ScheduleTemplates { get; set; }
            public List<SketchModel> Sketches { get; set; }

        }
        public class EditProfileViewModel
        {
            [Required(ErrorMessage = "Email обязателен")]
            [EmailAddress(ErrorMessage = "Неверный формат email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Номер телефона обязателен")]
            [Display(Name = "Номер телефона")]
            public string PhoneNumber { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Новый пароль")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Подтверждение пароля")]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            public string ConfirmPassword { get; set; }
        }
        public class SketchModel
        {
            public int id { get; set; }

            // Внешний ключ к тату-мастеру
            public int tattooistId { get; set; }

         
            public string imagePath { get; set; }
            public string description { get; set; }

            // Навигационное свойство (опционально)
            public TattooistModel Tattooist { get; set; }
        }
        public class SketchesViewModel
        {
            public List<SketchModel> Sketches { get; set; }
            public List<TattooistModel> Tattooists { get; set; }
            public int SelectedTattooistId { get; set; }
        }

        public class MainPageViewModel
        {
            public IEnumerable<TattooistModel> Tattooists { get; set; }
            public SalonInfo SalonInfos { get; set; }
           
        }
        // Шаблон слота, создаваемый администратором для конкретного тату-мастера
        public class ScheduleTemplate
        {
            public int id { get; set; }
            // Ссылка на тату-мастера
            public int tattooArtistId { get; set; }
            public int slotNumber { get; set; }  // например, 1, 2, 3...
            [Column(TypeName = "time")]
            public TimeSpan startTime { get; set; }
            public TattooistModel TattooArtist { get; set; }
        }
        public class ServicesModel
        {
            public int id { get; set; }
           
            public string name { get; set; }
            public string price { get; set; }
            public string description { get; set; }


        }
        // Запись пользователя на консультацию
        public class Appointment
        {
            public int id { get; set; }
            public int tattooArtistId { get; set; }
            // Дата, на которую бронируется слот
            public DateTime date { get; set; }
            // Номер слота согласно шаблону
            public int slotNumber { get; set; }
            // Ссылка на записавшегося пользователя
            public int userId { get; set; }
            public TattooistModel TattooArtist { get; set; }
            public UsersModel User { get; set; }
        }
        // ViewModel для страницы администратора
        public class AdminViewModel
        {
            public List<TattooistModel> TattooArtists { get; set; }
            public List<Appointment> Appointments { get; set; }
            // Данные для создания нового шаблона слотов
            public CreateSlotsViewModel CreateSlotsData { get; set; }
            public UsersModel CurrentUser { get; set; }
            public IEnumerable<ServicesModel> Services { get; set; }
            public List<UsersModel> Users { get; set; }
            // Добавляем коллекцию для информации о салоне
            public List<Tattoo_parlors.Models.SalonInfo> SalonInfos { get; set; }

        }

        // Данные для создания слотов
        public class CreateSlotsViewModel
        {
            public int TattooArtistId { get; set; }
            public int SlotCount { get; set; }
            // Список времён начала для каждого слота (длина списка должна соответствовать SlotCount)
            [Column(TypeName = "time")]
            public List<TimeSpan> SlotTimes { get; set; }
        }

        // ViewModel для страницы пользователя
        public class AppointmentViewModel
        {
            public List<TattooistModel> TattooArtists { get; set; }
            // Данные для бронирования
            public BookAppointmentViewModel BookingData { get; set; }
        }

        // Данные при бронировании слота
        public class BookAppointmentViewModel
        {
            public int TattooArtistId { get; set; }
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime Date { get; set; }
            public int SlotNumber { get; set; }
            // Обычно UserId получают из сессии/аутентификации, здесь для примера передаётся явно.
            public int UserId { get; set; }
            public List<TattooistModel> TattooArtists { get; set; }
        }
    }
}
