using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;

namespace DSTT_Backend.Services.IServices
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsers();
        Task<UserModel?> GetUser(int userId);

        Task<UserModel?> GetUser(string username);

        Task<ServiceIDOperationResult> CreateUser(UserDTO user);

        Task<ServiceOperationResult> EditUser(UserDTO updatedUser, int userId);

        Task<ServiceOperationResult> DeleteUser(int userId);
    }
}
