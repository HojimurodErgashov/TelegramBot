using System.Linq.Expressions;
using TelegramBot.Products.Entity;
using TelegramBot.Products.Interfaces;
using TelegramBot.Repository;

namespace TelegramBot.Products.Service
{
    public class ProductService : IProductService
    {

        private readonly IGenericRepository<Product> _productManager;

        public ProductService(IGenericRepository<Product> productManager)
            => _productManager = productManager;

        public async ValueTask<Product> CreateProductAsync(Product product)
        {
            var newProduct = new Product();
            if (product != null)
            {
                newProduct = await _productManager.CreateAsync(product);
            }

            await _productManager.SaveChangeAsync();

            return newProduct;
        }

        public async ValueTask<bool> DeleteProductAsync(long Id)
            => await _productManager.DeleteAsync(Id);

        public async ValueTask<IQueryable<Product>> GetAllProductAsync(Expression<Func<Product, bool>> expression, string[] includes = null, bool isTracking = true)
            => _productManager.GetAllAsync(expression, includes, isTracking);

        public async ValueTask<Product> GetProductAsync(long? Id)
            => await _productManager.GetAsync(p => p.Id == Id, false, null);

        public async ValueTask<Product> UpdateProduct(Product product)
        {
            var productData = await GetProductAsync(product.Id);

            if (productData is null)
            {
                return null;
            }

            productData.Name_uz = productData.Name_uz;
            productData.Name_en = productData.Name_en;

            var productUpdate = _productManager.Update(productData);
            await _productManager.SaveChangeAsync();
            return await Task<Product>.FromResult(productUpdate);
        }


    }
}
