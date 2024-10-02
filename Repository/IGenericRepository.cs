using System.Linq.Expressions;
using TelegramBot.AuditableModel;

namespace TelegramBot.Repository
{
    public interface IGenericRepository<T>  where T : BaseEntity
    {
        ValueTask<T> CreateAsync(T entity);
        IQueryable<T> GetAllAsync(Expression<Func<T, bool>> expression, string[] includes = null, bool isTracking = true);
        ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, bool istracking = true, string[] includes = null);
        T Update(T entity);
        ValueTask<bool> DeleteAsync(long id);
        ValueTask SaveChangeAsync();
    }
}
