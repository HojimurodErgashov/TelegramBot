using System.Collections.Generic;
using System.Linq.Expressions;
using TelegramBot.Users.Entity;

namespace TelegramBot.Users.Interfaces
{
    public interface IProductService
    {
        ValueTask<User> CreateUserAsync(User user);
        ValueTask<User> GetUserAsync(long? Id);
        ValueTask<User> UpdateUser(User user);
        ValueTask<bool> DeleteUserAsync(long Id);
        ValueTask<IQueryable<User>> GetAllUserAsync(Expression<Func<User, bool>> expression, string[] includes = null, bool isTracking = true);
    }
}
