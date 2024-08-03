using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.InMemory;

namespace DSTT_Test.RepositoriesTests
{
    public class UserRepositoryTest : BaseRepositoryTest
    {
        private readonly UserRepository _userRepository;

        public UserRepositoryTest() : base()
        {
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task GetUsers_ReturnsAllUsers()
        {
            ClearDatabase();

            var users = new List<UserDTO>
            {
                new UserDTO { Username = "User1" },
                new UserDTO { Username = "User2" },
                new UserDTO { Username = "User3" }
            };

            foreach (var user in users)
            {
                await _userRepository.CreateUser(user);
            }


            var result = await _userRepository.GetUsers();

            Assert.Equal(users.Count, result.Count);
            Assert.All(result, u => Assert.Contains(u.Username, users.Select(x => x.Username)));
        }

        [Fact]
        public async Task GetUsers_EmptyDatabase_ReturnsEmptyList()
        {
            ClearDatabase();

            var result = await _userRepository.GetUsers();

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateUserAndGetUserById_Success()
        {
            ClearDatabase();

            var user = new UserDTO
            {
                Username = "TestIDUser"
            };

            var userId = await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserById_NonExistentID_ReturnsNull()
        {
            ClearDatabase();
            var result = await _userRepository.GetUserById(999999);
            Assert.Null(result);

        }

        [Fact]
        public async Task CreateUserAndGetUserByName_Success()
        {
            ClearDatabase();
            var user = new UserDTO
            {
                Username = "TestNameUser"
            };

            await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserByName(user.Username);

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);


        }

        [Fact]
        public async Task GetUserByName_NonExistentName_ReturnsNull()
        {
            ClearDatabase();
            var result = await _userRepository.GetUserByName("NonExistentName");
            Assert.Null(result);

        }

        [Fact]
        public async Task EditUser_Success()
        {
            ClearDatabase();
            var user = new UserDTO
            {
                Username = "TestEditUser"
            };

            var userId = await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);

            var updatedUser = new UserDTO
            {
                Username = "TestUpdateUser"
            };

            var editResult = await _userRepository.EditUser(updatedUser, result);

            Assert.True(editResult.Success);


        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            ClearDatabase();

            var user = new UserDTO
            {
                Username = "TestDeleteUser"
            };

            var userId = await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);

            var deleteResult = await _userRepository.DeleteUser(result);

            Assert.True(deleteResult.Success);

            var deletedUser = await _userRepository.GetUserById(userId);
            Assert.Null(deletedUser);


        }

        [Fact]
        public async Task DeleteUser_NonExistentUser_ReturnsError()
        {
            ClearDatabase();

            var deleteResult = await _userRepository.DeleteUser(new User());

            Assert.False(deleteResult.Success);

        }

    }
}