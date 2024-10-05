using TelegramBot.AuditableModel;

namespace TelegramBot.Modifies.Entities
{
    public class Modify : BaseEntity
    {
        public string? Name_uz { get; set; }
        public string? Name_en { get; set; }
        public double? Price { get; set; }
    }
}