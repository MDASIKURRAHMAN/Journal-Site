using eJournal.Domain.Models;

namespace eJournal.Services.Interfaces
{
    public interface IUserService
    {
        Task UpdateUser(User user);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<bool> IfUserNameExist(string userName, int userId);
        Task<IAsyncEnumerable<User>> GetAllUserAsync();
    }
}
