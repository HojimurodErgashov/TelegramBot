using System.Linq.Expressions;
using TelegramBot.Categories.Entity;
using TelegramBot.Categories.Interfaces;
using TelegramBot.Repository;

namespace TelegramBot.Categories.Services
{
    public class CategoryService:ICateogryService
    {
        private readonly IGenericRepository<Category> _categoryManager;

        public CategoryService(IGenericRepository<Category> userManager)
            => _categoryManager = userManager;

        public async ValueTask<Category> CreateCategoryAsync(Category category)
        {
            var registerUser = new Category();
            if (category != null)
            {
                registerUser = await _categoryManager.CreateAsync(category);
            }

            await _categoryManager.SaveChangeAsync();

            return registerUser;
        }

        public async ValueTask<bool> DeleteCategoryAsync(long Id)
            => await _categoryManager.DeleteAsync(Id);

        public async ValueTask<IQueryable<Category>> GetAllCategoryAsync(Expression<Func<Category, bool>> expression, string[] includes = null, bool isTracking = true)
            => _categoryManager.GetAllAsync(expression, includes, isTracking);

        public async ValueTask<Category> GetCategoryAsync(long? Id)
            => await _categoryManager.GetAsync(p => p.Id == Id, false, null);

        public async ValueTask<Category> UpdateCategory(Category category)
        {
            var categoryData = await GetCategoryAsync(category.Id);

            if (categoryData is null)
            {
                return null;
            }

            categoryData.Name_uz = category.Name_uz;
            categoryData.Name_en = category.Name_en;

            var categoryUpdate = _categoryManager.Update(categoryData);
            await _categoryManager.SaveChangeAsync();
            return await Task<Category>.FromResult(categoryUpdate);
        }
    }
}
