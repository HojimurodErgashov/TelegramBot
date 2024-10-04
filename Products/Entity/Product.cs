using TelegramBot.AuditableModel;
using TelegramBot.Modifies;

namespace TelegramBot.Products.Entity
{
    public class Product : BaseEntity
    {
        public string? Name_uz { get; set; }
        public string? Name_en { get; set; }
        public double? Price { get; set; }
        public List<Modify>? Modifies { get; set; }
    }
}
