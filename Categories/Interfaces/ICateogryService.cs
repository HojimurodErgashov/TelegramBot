using System.Linq.Expressions;
using TelegramBot.Categories.Entity;

namespace TelegramBot.Categories.Interfaces
{
    public interface ICateogryService
    {
        ValueTask<Category> CreateCategoryAsync(Category category);
        ValueTask<Category> GetCategoryAsync(long? Id);
        ValueTask<Category> UpdateCategory(Category category);
        ValueTask<bool> DeleteCategoryAsync(long Id);
        ValueTask<IQueryable<Category>> GetAllCategoryAsync(Expression<Func<Category, bool>> expression, string[] includes = null, bool isTracking = true);
    }
}
