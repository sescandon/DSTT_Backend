using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DSTT_Test.RepositoriesTests
{
    public class UserRepositoryTest
    {
        private readonly DsttDbContext _context;
        private readonly UserRepository _userRepository;

        public UserRepositoryTest()
        {
            string testDb = Secret.TestDBConnectionString;
            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateUserAndGetUserById_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO
            {
                Username = "TestIDUser"
            };

            var userId = await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            await transaction.RollbackAsync();
        }



        [Fact]
        public async Task GetUserById_NonExistentID_ReturnsNull()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var result = await _userRepository.GetUserById(999999);
            Assert.Null(result);
            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task CreateUserAndGetUserByName_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO
            {
                Username = "TestNameUser"
            };

            await _userRepository.CreateUser(user);
            var result = await _userRepository.GetUserByName(user.Username);

            Assert.NotNull(result);
            Assert.Equal(user.Username, result.Username);
            await transaction.RollbackAsync();

        }

        [Fact]
        public async Task GetUserByName_NonExistentName_ReturnsNull()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var result = await _userRepository.GetUserByName("NonExistentName");
            Assert.Null(result);
            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task EditUser_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
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

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

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

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task DeleteUser_NonExistentUser_ReturnsError()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var deleteResult = await _userRepository.DeleteUser(new User());

            Assert.False(deleteResult.Success);

            await transaction.RollbackAsync();
        }

    }
}