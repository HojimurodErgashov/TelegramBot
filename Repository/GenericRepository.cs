using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using TelegramBot.AuditableModel;
using TelegramBot.BotDbCOntext;

namespace TelegramBot.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly BotDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(BotDbContext botDbContext)
        {
            _context = botDbContext;
            _dbSet = _context.Set<T>();
        }

        public async ValueTask<T> CreateAsync(T entity)
            => (await _context.AddAsync(entity)).Entity;

        public async ValueTask<bool> DeleteAsync(long id)
        {
            var entity = await GetAsync(x => x.Id == id);
            if(entity == null)
            {
                    return false;
            }
            _dbSet.Remove(entity);
            return true;
        }

        public IQueryable<T> GetAllAsync(Expression<Func<T, bool>> expression, string[] includes = null, bool isTracking = true)
        {
            var query = expression is null ? _dbSet : _dbSet.Where(expression);

            if (includes != null)
                foreach (var include in includes)
                    if (!string.IsNullOrEmpty(include))
                        query = query.Include(include);

            if (!isTracking)
                query = query
                    .AsTracking()
                    .AsSplitQuery();
            return query;

        }

        public async ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, bool istracking = true, string[] includes = null)
            => await GetAllAsync(expression, includes, istracking).FirstOrDefaultAsync();

        public async ValueTask SaveChangeAsync()
            => await _context.SaveChangesAsync();

        public T Update(T entity)
            => _dbSet.Update(entity).Entity;
    }
}
