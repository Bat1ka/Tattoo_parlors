namespace Tattoo_parlors.Models
{
    public class MessageModel
    {
        public int id { get; set; }
        public string content { get; set; }
        // Идентификатор отправителя
        public int senderId { get; set; }
        public UsersModel Sender { get; set; }

        // Идентификатор получателя (может быть null, если сообщение "массовое")
        public int? receiverId { get; set; }
        public UsersModel Receiver { get; set; }

        public DateTime timestamp { get; set; } = DateTime.Now;
    }
}
