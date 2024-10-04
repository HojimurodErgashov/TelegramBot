using System.Linq.Expressions;
using TelegramBot.Repository;
using TelegramBot.Users.Entity;
using TelegramBot.Users.Interfaces;

namespace TelegramBot.Users.Services
{
    public class UserServices : IProductService
    {
            private readonly IGenericRepository<User> _userManager;

            public UserServices(IGenericRepository<User> userManager) 
                => _userManager = userManager;

            public async ValueTask<User> CreateUserAsync(User user)
            {
                var registerUser = new User();
                if (user != null)
                {
                     registerUser = await _userManager.CreateAsync(user);
                }

                await _userManager.SaveChangeAsync();

                return registerUser;
            }

            public async ValueTask<bool> DeleteUserAsync(long Id)
                =>await _userManager.DeleteAsync(Id);

            public async ValueTask<IQueryable<User>> GetAllUserAsync(Expression<Func<User, bool>> expression, string[] includes = null, bool isTracking = true)
                =>  _userManager.GetAllAsync(expression, includes, isTracking);

            public async ValueTask<User> GetUserAsync(long? Id)
                => await _userManager.GetAsync(p => p.Id == Id, false, null);

            public async ValueTask<User> UpdateUser(User user)
            {
                var userData = await GetUserAsync(user.Id);

                if (userData != null) 
                {
                    return null;
                }

                userData.PhoneNumber = user.PhoneNumber;
                userData.FullName = user.FullName;
                userData.password = user.password;
                userData.Email = user.Email;
                userData.Code = user.Code;
                userData.CurrentState = user.CurrentState;
                userData.LanguageCode = user.LanguageCode;

               var userUpdate = _userManager.Update(userData);
                await _userManager.SaveChangeAsync();
                return await Task<User>.FromResult(userUpdate);
            }

            public async Task<string?> GetUserLanguageCodeAsync(long? userId)
            {
                var user = await GetUserAsync(userId);
                return user?.LanguageCode;
            }

            public async Task<(bool IsSuccess, string? ErrorMessage)> UpdateLanguageCodeAsync(long userId , string languageCode)
            {
                ArgumentNullException.ThrowIfNull(languageCode);
                var user = await GetUserAsync(userId);
                if (user is null) 
                {
                    return (false, "User not found!");
                }
                user.LanguageCode = languageCode;
                await UpdateUser(user);
                return (true, "Update Language");
            }
    }
}
