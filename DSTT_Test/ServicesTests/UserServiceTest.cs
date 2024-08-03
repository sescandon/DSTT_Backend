using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services;

namespace DSTT_Test.ServicesTests
{
    public class UserServiceTest 
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly UserService _userService;

        public UserServiceTest()
        {
            _userRepository = new Mock<IUserRepository>();
            _userService = new UserService(_userRepository.Object);
        }

        [Fact]
        public async Task CreateUser_Success()
        {
            
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.CreateUser(user)).ReturnsAsync(1);

            var result = await _userService.CreateUser(user);
            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            Assert.Null(result.ErrorMessage);
            Assert.Null(result.StatusCode);
            _userRepository.Verify(repo => repo.CreateUser(user), Times.Once);
        }

        [Fact]
        public async Task CreateUser_DatabaseFail()
        {
            
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.CreateUser(user)).ThrowsAsync(new Exception("Database error"));

            var result = await _userService.CreateUser(user);
            Assert.False(result.Success);
            Assert.Null(result.Id);
            Assert.Contains("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.CreateUser(user), Times.Once);
        }

        [Fact]
        public async Task CreateUser_ExistingUserFail()
        {
            
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            var existingUser = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserByName(user.Username)).ReturnsAsync(existingUser);

            var result = await _userService.CreateUser(user);
            Assert.False(result.Success);
            Assert.Null(result.Id);
            Assert.Contains("User already exists", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserByName(user.Username), Times.Once);
        }




        [Fact]
        public async Task GetUserId_Fail()
        {

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null!);

            var userResult = await _userService.GetUser(1);
            Assert.Null(userResult);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);

        }

        [Fact]
        public async Task GetUserId_Success()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);

            var userResult = await _userService.GetUser(1);
            Assert.NotNull(userResult);
            Assert.Equal(1, userResult.Id);
            Assert.Equal("TestUser", userResult.Username);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);

        }

        [Fact]
        public async Task GetUserName_Fail()
        {

            _userRepository.Setup(repo => repo.GetUserByName("TestUser")).ReturnsAsync((User)null!);

            var userResult = await _userService.GetUser("TestUser");
            Assert.Null(userResult);
            _userRepository.Verify(repo => repo.GetUserByName("TestUser"), Times.Once);
        }

        [Fact]
        public async Task GetUserName_Success()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserByName("TestUser")).ReturnsAsync(user);

            var userResult = await _userService.GetUser("TestUser");
            Assert.NotNull(userResult);
            Assert.Equal(1, userResult.Id);
            Assert.Equal("TestUser", userResult.Username);
            _userRepository.Verify(repo => repo.GetUserByName("TestUser"), Times.Once);

        }


        [Fact]
        public async Task EditUser_Success()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            var userDTO = new UserDTO
            {
                Username = "TestUser2"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);
            _userRepository.Setup(repo => repo.EditUser(userDTO, user)).ReturnsAsync(new BasicOperationResult { Success = true });

            var result = await _userService.EditUser(userDTO, user.Id);
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Null(result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.EditUser(userDTO, user), Times.Once);
        }

        [Fact]
        public async Task EditUser_UserNotFound()
        {
            var userDTO = new UserDTO
            {
                Username = "TestUser2"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null!);

            var result = await _userService.EditUser(userDTO, 1);
            Assert.False(result.Success);
            Assert.Contains("User doesn't exist", result.ErrorMessage);
            Assert.Equal(404, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.EditUser(userDTO, It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task EditUser_UserAlreadyHasUsername()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            var userDTO = new UserDTO
            {
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);

            var result = await _userService.EditUser(userDTO, user.Id);
            Assert.False(result.Success);
            Assert.Contains("User already has this username", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.EditUser(userDTO, It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task EditUser_DatabaseFail()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            var userDTO = new UserDTO
            {
                Username = "TestUser2"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var result = await _userService.EditUser(userDTO, user.Id);
            Assert.False(result.Success);
            Assert.Contains("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task EditUser_EditInDatabaseFail()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            var userDTO = new UserDTO
            {
                Username = "TestUser2"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);
            _userRepository.Setup(repo => repo.EditUser(userDTO, user)).ReturnsAsync(new BasicOperationResult { Success = false, ErrorMessage = "Database error" });

            var result = await _userService.EditUser(userDTO, user.Id);
            Assert.False(result.Success);
            Assert.Contains("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.EditUser(userDTO, user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);
            _userRepository.Setup(repo => repo.DeleteUser(user)).ReturnsAsync(new BasicOperationResult { Success = true });

            var result = await _userService.DeleteUser(user.Id);
            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Null(result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.DeleteUser(user), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null!);

            var result = await _userService.DeleteUser(1);
            Assert.False(result.Success);
            Assert.Contains("User doesn't exist", result.ErrorMessage);
            Assert.Equal(404, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.DeleteUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task DeleteUser_DatabaseFail()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var result = await _userService.DeleteUser(user.Id);
            Assert.False(result.Success);
            Assert.Contains("Database error", result.ErrorMessage);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_DeleteInDatabaseFail()
        {
            var user = new User
            {
                Id = 1,
                Username = "TestUser"
            };

            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(user);
            _userRepository.Setup(repo => repo.DeleteUser(user)).ReturnsAsync(new BasicOperationResult { Success = false, ErrorMessage = "Database error" });

            var result = await _userService.DeleteUser(user.Id);
            Assert.False(result.Success);
            Assert.Contains("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.DeleteUser(user), Times.Once);

        }

    }
}
