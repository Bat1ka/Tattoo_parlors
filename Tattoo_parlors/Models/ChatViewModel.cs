namespace Tattoo_parlors.Models
{
    public class ChatViewModel
    {
        public List<MessageModel> Messages { get; set; } = new List<MessageModel>();

        // Список пользователей для выбора получателя (только для администратора)
        public List<UsersModel> Users { get; set; } = new List<UsersModel>();

        // Текст сообщения, вводимый пользователем  
        public string MessageContent { get; set; }

        // Выбранный получатель для сообщения администратора
        public int? SelectedUserId { get; set; }

        // Роль текущего пользователя (0 или 1)
        public int CurrentUserRole { get; set; }
        public int CurrentUserId { get; set; }
    }
}
