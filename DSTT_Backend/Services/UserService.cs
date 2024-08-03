using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services.IServices;

namespace DSTT_Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            List<User> users = await _userRepository.GetUsers();
            List<UserModel> userModels = new();

            foreach (User user in users)
            {
                userModels.Add(new UserModel
                {
                    Id = user.Id,
                    Username = user.Username
                });
            }

            return userModels;
        }

        public async Task<UserModel?> GetUser(int userId)
        {
            User? user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return null;
            }

            return new UserModel
            {
                Id = user.Id,
                Username = user.Username
            };
        }

        public async Task<UserModel?> GetUser(string username)
        {
            User? user = await _userRepository.GetUserByName(username);
            if (user == null)
            {
                return null;
            }

            return new UserModel
            {
                Id = user.Id,
                Username = user.Username
            };
        }

        public async Task<ServiceIDOperationResult> CreateUser(UserDTO user)
        {
            try
            {
                User? potentialUser = await _userRepository.GetUserByName(user.Username);

                if (potentialUser != null)
                {
                    return new ServiceIDOperationResult { 
                        Success = false,
                        ErrorMessage = "User already exists",
                        StatusCode = 400
                    };
                }

                int newUserId = await _userRepository.CreateUser(user);

                return new ServiceIDOperationResult
                {
                    Success = true,
                    Id = newUserId
                };
            }
            catch (Exception ex)
            {

                return new ServiceIDOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                    
                };
            }


        }

        public async Task<ServiceOperationResult> EditUser(UserDTO updatedUser, int userId)
        {
            try
            {
                User? existingUser = await _userRepository.GetUserById(userId);
                if (existingUser == null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                if(existingUser.Username == updatedUser.Username)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User already has this username",
                        StatusCode = 400
                    };
                }

                BasicOperationResult result = await _userRepository.EditUser(updatedUser, existingUser);

                if(!result.Success)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = result.ErrorMessage,
                        StatusCode = 500
                    };
                }

                return new ServiceOperationResult
                {
                    Success = true,
                };
            }
            catch (Exception ex)
            {

                return new ServiceOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }


            
        }

        public async Task<ServiceOperationResult> DeleteUser(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                BasicOperationResult result = await _userRepository.DeleteUser(user);

                if (!result.Success)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = result.ErrorMessage,
                        StatusCode = 500
                    };
                }

                return new ServiceOperationResult
                {
                    Success = true,
                };

            }
            catch (Exception ex)
            {

                return new ServiceOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
            

            
        }
    }
}
