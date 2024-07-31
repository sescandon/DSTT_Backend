using DSTT_Backend.Database;
using DSTT_Backend.Models;
using DSTT_Backend.Models.User;

namespace DSTT_Backend.Repositories.IRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserById(int userId);

        Task<User?> GetUserByName(string username);

        Task<List<User>> GetUsers();

        Task<int> CreateUser(UserDTO user);

        Task<OperationResult> EditUser(UserDTO updatedUser, User existingUser);

        Task<OperationResult> DeleteUser(User user);

    }
}
