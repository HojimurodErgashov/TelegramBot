using Microsoft.EntityFrameworkCore;
using TelegramBot.Categories.Entity;
using TelegramBot.Modifies;
using TelegramBot.Products;
using TelegramBot.Users.Entity;
using TelegramBot.Carts;

namespace TelegramBot.BotDbCOntext
{
    public class BotDbContext:DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Modify> Modifies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartObject> CartObjects { get; set; }
    }
}