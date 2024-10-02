using TelegramBot.AuditableModel;
using TelegramBot.Products;

namespace TelegramBot.Categories.Entity
{
    public class Category:BaseEntity
    {
        public string? Name_uz {  get; set; }
        public string? Name_en {  get; set; }
        public List<Product>? Products { get; set; }
    }
}