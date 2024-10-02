using TelegramBot.AuditableModel;

namespace TelegramBot.Carts
{
    public class CartObject:BaseEntity
    {
        public string? ProductName {  get; set; }
        public int Count {  get; set; }
    }
}