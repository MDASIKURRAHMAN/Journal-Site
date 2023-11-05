using eJournal.Domain.Models;
using eJournal.Repository;
using eJournal.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace eJournal.Services.Implementions
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> GetUserById(int id)
        {
            var result = await _userRepository.GetByIdAsync(id);
            if (result != null)
            {
                return result;
            }
            return null;
        }
        public async Task<bool> IfUserNameExist(string userName, int userId)
        {
            var result = await _userRepository.GeneralSearch(x => x.UserName == userName && x.UserId != userId).ToListAsync();
            if (result.Count > 0)
            {
                return true;
            }
            return false;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            var result = await _userRepository.GeneralSearch(x => x.UserEmail == email).ToListAsync();
            if (result.Count > 0)
            {
                return result[0];
            }
            return null;
        }

        public async Task UpdateUser(User user)
        {
            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while updating the user:" + ex);
            }
            return;
        }
        public async Task<IAsyncEnumerable<User>> GetAllUserAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }
}