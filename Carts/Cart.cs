using TelegramBot.AuditableModel;

namespace TelegramBot.Carts
{
    public class Cart:BaseEntity
    {
        public long UserId {  get; set; }
        public List<CartObject> carts { get; set; }
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}