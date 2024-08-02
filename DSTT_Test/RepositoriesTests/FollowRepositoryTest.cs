using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using DSTT_Test;


namespace DSTT_Test.RepositoriesTests
{
    public class FollowRepositoryTest
    {
        private readonly DsttDbContext _context;
        private readonly UserRepository _auxiliaryUserRepository;
        private readonly FollowRepository _followRepository;

        public FollowRepositoryTest()
        {
            string testDb = Secret.TestDBConnectionString;

            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _followRepository = new FollowRepository(_context);
            _auxiliaryUserRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task FollowUser_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestFollowUser1" };
            var user2 = new UserDTO { Username = "TestFollowUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            var result = await _followRepository.FollowUser(userId1, userId2);

            Assert.True(result.Success);

            await transaction.RollbackAsync();
        }

        

        [Fact]
        public async Task UnFollowUser_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestUnfollowUser1" };
            var user2 = new UserDTO { Username = "TestUnfollowUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            await _followRepository.FollowUser(userId1, userId2);
            var follow = await _followRepository.IsFollowing(userId1, userId2);

            var result = await _followRepository.UnFollowUser(follow);

            Assert.True(result.Success);

            await transaction.RollbackAsync();
        }
        

        [Fact]
        public async Task IsFollowing_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestisFollowingUser1" };
            var user2 = new UserDTO { Username = "TestisFollowingUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            await _followRepository.FollowUser(userId1, userId2);
            var follow = await _followRepository.IsFollowing(userId1, userId2);

            Assert.NotNull(follow);
            Assert.Equal(userId1, follow.FollowerId);
            Assert.Equal(userId2, follow.FollowedId);

            await transaction.RollbackAsync();
        }

        
        [Fact]
        public async Task GetFollowers_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestFollowersUser1" };
            var user2 = new UserDTO { Username = "TestFollowersUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            await _followRepository.FollowUser(userId1, userId2);
            var followers = await _followRepository.GetFollowers(userId2);

            Assert.Single(followers);
            Assert.Equal(userId1, followers[0].Id);


            await transaction.RollbackAsync();
        }

        

        [Fact]
        public async Task GetFollowers_NoFollowers_ReturnsEmptyList()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestGetFollowersUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var followers = await _followRepository.GetFollowers(userId);

            Assert.Empty(followers);

            await transaction.RollbackAsync();
        }

        

        [Fact]
        public async Task GetFollowing_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestFollowingUser1" };
            var user2 = new UserDTO { Username = "TestFollowingUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            await _followRepository.FollowUser(userId1, userId2);
            var followed = await _followRepository.GetFollowing(userId1);

            Assert.Single(followed);
            Assert.Equal(userId2, followed[0].Id);

            await transaction.RollbackAsync();
        }

        

        [Fact]
        public async Task GetFollowing_NoFollowed_ReturnsEmptyList()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestGetFollowingUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var followed = await _followRepository.GetFollowing(userId);

            Assert.Empty(followed);

            await transaction.RollbackAsync();
        }

    }
}
