using System.Linq.Expressions;
using TelegramBot.Products.Entity;

namespace TelegramBot.Products.Interfaces
{
    public interface IProductService
    {
        ValueTask<Product> CreateProductAsync(Product product);
        ValueTask<Product> GetProductAsync(long? Id);
        ValueTask<Product> UpdateProduct(Product product);
        ValueTask<bool> DeleteProductAsync(long Id);
        ValueTask<IQueryable<Product>> GetAllProductAsync(Expression<Func<Product, bool>> expression, string[] includes = null, bool isTracking = true);
        ValueTask<IQueryable<Product>> GetAllProductsByCategoryIdAsync(long Id);
    }
}