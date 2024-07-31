using DSTT_Backend.Database;
using DSTT_Backend.Models;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DSTT_Backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DsttDbContext _dbcontext;

        public UserRepository(DsttDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<User?> GetUserById(int userId)
        {
            User? potentialUser = await _dbcontext.Users.FindAsync(userId);
            return potentialUser;
        }

        public async Task<User?> GetUserByName(string username)
        {
            User? potentialUser = await _dbcontext.Users.FirstOrDefaultAsync(u => u.Username == username);
            return potentialUser;
        }

        public async Task<List<User>> GetUsers()
        {
            List<User> userList = await _dbcontext.Set<User>().ToListAsync();
            return userList;
        }

        public async Task<int> CreateUser(UserDTO user)
        {
            User newUser = new()
            {
                Username = user.Username
            };

            await _dbcontext.Users.AddAsync(newUser);
            await _dbcontext.SaveChangesAsync();

            return newUser.Id;
        }

        public async Task<OperationResult> EditUser(UserDTO updatedUser, User existingUser)
        {
            try
            {
                existingUser.Username = updatedUser.Username;
                await _dbcontext.SaveChangesAsync();
                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {

                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }

        }

        public async Task<OperationResult> DeleteUser(User user)
        {
            try
            {
                _dbcontext.Users.Remove(user);
                await _dbcontext.SaveChangesAsync();
                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {

                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

    }
}
