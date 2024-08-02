using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DSTT_Test.ServicesTests
{
    public class UserServiceTest
    {
        private readonly UserService _userService;
        private readonly DsttDbContext _context;
        private readonly UserRepository _userRepository;

        public UserServiceTest()
        {
            string testDb = "Server=localhost;Database=DSTT_DB_TEST;Trusted_Connection=true;TrustServerCertificate=true";
            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _userRepository = new UserRepository(_context);
            _userService = new UserService(_userRepository);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateUserAndGetUser_Test(bool shouldSucceed)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            var result = await _userService.CreateUser(user);
            Assert.True(result.Success);

            var userResult = await _userService.GetUser("TestUser");
            Assert.NotNull(userResult);
            Assert.Equal("TestUser", userResult.Username);

            if (!shouldSucceed)
            {
                var result2 = await _userService.CreateUser(user);
                Assert.False(result2.Success);
            }

            await transaction.RollbackAsync();
        }


        [Fact]
        public async Task GetUser_Fail()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var userResult = await _userService.GetUser("TestUser");
            Assert.Null(userResult);

            await transaction.RollbackAsync();
        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetUserById_Test(bool shouldSucceed)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            var result = await _userService.CreateUser(user);
            Assert.True(result.Success);

            var userId = result.Id;

            Assert.NotNull(userId);

            var userResult = await _userService.GetUser(userId.Value);
            Assert.NotNull(userResult);
            Assert.Equal("TestUser", userResult.Username);

            if (!shouldSucceed)
            {
                var userResult2 = await _userService.GetUser(9999);
                Assert.Null(userResult2);
            }

            await transaction.RollbackAsync();
        }


        [Theory]
        [InlineData("TestUser", "UpdatedUser", true)]
        [InlineData("TestUser", "UpdatedUser", false)]
        [InlineData("TestUser", "TestUser", true)]
        public async Task EditUser_Test(string initialUsername, string updatedUsername, bool exists)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO { Username = initialUsername };
            var result = await _userService.CreateUser(user);
            Assert.True(result.Success);
            var userId = result.Id;
            Assert.NotNull(userId);

            var userResult = await _userService.GetUser(userId.Value);
            Assert.NotNull(userResult);
            Assert.Equal(initialUsername, userResult.Username);

            var updatedUser = new UserDTO { Username = updatedUsername };

            if (initialUsername != updatedUsername) {
                if (!exists)
                {
                    var editResult2 = await _userService.EditUser(updatedUser, 9999);
                    Assert.False(editResult2.Success);
                }
                else
                {
                    var editResult = await _userService.EditUser(updatedUser, userId.Value);
                    Assert.True(editResult.Success);

                    var updatedUserResult = await _userService.GetUser(userId.Value);
                    Assert.NotNull(updatedUserResult);
                    Assert.Equal(updatedUsername, updatedUserResult.Username);
                }
            }
            else
            {
                var editResult = await _userService.EditUser(updatedUser, userId.Value);
                Assert.False(editResult.Success);
            }
           
            await transaction.RollbackAsync();

        }


        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteUser_Test(bool shouldSucceed)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            var user = new UserDTO
            {
                Username = "TestUser"
            };

            var result = await _userService.CreateUser(user);
            Assert.True(result.Success);

            var userId = result.Id;
            Assert.NotNull(userId);

            var userResult = await _userService.GetUser(userId.Value);
            Assert.NotNull(userResult);
            Assert.Equal("TestUser", userResult.Username);

            if (shouldSucceed)
            {
                var deleteResult = await _userService.DeleteUser(userId.Value);
                Assert.True(deleteResult.Success);

                var userResult2 = await _userService.GetUser(userId.Value);
                Assert.Null(userResult2);
            }
            else
            {
                var deleteResult = await _userService.DeleteUser(9999);
                Assert.False(deleteResult.Success);
            }

            await transaction.RollbackAsync();
        }


    }
}
