using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


namespace DSTT_Test.ServicesTests
{
    public class FollowServiceTest
    {
        private readonly FollowService _followService;
        private readonly DsttDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly FollowRepository _followRepository;

        public FollowServiceTest()
        {
            string testDb = Secret.TestDBConnectionString;
            var options = new DbContextOptionsBuilder<DsttDbContext>()
            .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _userRepository = new UserRepository(_context);
            _followRepository = new FollowRepository(_context);
            _followService = new FollowService(_followRepository, _userRepository);
        }

        [Theory]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, false)]
        public async Task FollowUser_Test(bool followerExists, bool followeeExists, bool sameUser, bool following)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            int followerId, followeeId;

            if (followerExists)
            {
                var follower = new UserDTO
                {
                    Username = "Follower"
                };
                var createdResult = await _userRepository.CreateUser(follower);
                Assert.True(createdResult > 0);
                followerId = createdResult;

                if (sameUser)
                {
                    followeeId = followerId;
                    var followResult = await _followService.FollowUser(followerId, followeeId);
                    Assert.False(followResult.Success);
                }
            }
            else
            {
                followerId = 9999;
            }

            if (followeeExists)
            {

                var followee = new UserDTO
                {
                    Username = "Followee"
                };
                var result = await _userRepository.CreateUser(followee);
                Assert.True(result > 0);
                followeeId = result;

            }
            else
            {
                followeeId = 9998;
            }

            if (!followeeExists  || !followerExists )
            {
                var followResult = await _followService.FollowUser(followerId, followeeId);
                Assert.False(followResult.Success);

            } else
            {
                if (following)
                {
                    var followResult = await _followService.FollowUser(followerId, followeeId);
                    Assert.True(followResult.Success);
                    var followAgainResult = await _followService.FollowUser(followerId, followeeId);
                    Assert.False(followAgainResult.Success);
                }
                else
                {
                    var followResult = await _followService.FollowUser(followerId, followeeId);
                    Assert.True(followResult.Success);
                }
            }

            await transaction.RollbackAsync();

        }

        [Theory]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, true)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, false)]
        public async Task UnFollowUser_Test(bool followerExists, bool followeeExists, bool sameUser, bool following)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            int followerId, followeeId;

            if (followerExists)
            {
                var follower = new UserDTO
                {
                    Username = "Follower"
                };
                var createdResult = await _userRepository.CreateUser(follower);
                Assert.True(createdResult > 0);
                followerId = createdResult;

                if (sameUser)
                {
                    followeeId = followerId;
                    var followResult = await _followService.UnFollowUser(followerId, followeeId);
                    Assert.False(followResult.Success);
                }
            }
            else
            {
                followerId = 9999;
            }

            if (followeeExists)
            {

                var followee = new UserDTO
                {
                    Username = "Followee"
                };
                var result = await _userRepository.CreateUser(followee);
                Assert.True(result > 0);
                followeeId = result;

            }
            else
            {
                followeeId = 9998;
            }

            if (!followeeExists || !followerExists)
            {
                var followResult = await _followService.UnFollowUser(followerId, followeeId);
                Assert.False(followResult.Success);

            }
            else
            {
                if (following)
                {
                    var followResult = await _followService.FollowUser(followerId, followeeId);
                    Assert.True(followResult.Success);
                    var followAgainResult = await _followService.UnFollowUser(followerId, followeeId);
                    Assert.True(followAgainResult.Success);
                }
                else
                {
                    var followResult = await _followService.UnFollowUser(followerId, followeeId);
                    Assert.False(followResult.Success);
                }
            }

            await transaction.RollbackAsync();

        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, false)]
        
        public async Task GetFollowers_Test(bool userExists, bool hasFollowers)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (userExists)
            {
                UserDTO user = new()
                {
                    Username = "TestUser"
                };
                var userId = await _userRepository.CreateUser(user);

                if (hasFollowers)
                {
                    UserDTO follower = new()
                    {
                        Username = "Follower"
                    };
                    var followerId = await _userRepository.CreateUser(follower);

                    var followResult = await _followService.FollowUser(followerId, userId);
                    Assert.True(followResult.Success);

                    var followers = await _followService.GetFollowers(userId);
                    Assert.True(followers.Success);
                    Assert.NotEmpty(followers.Data!);
                }
                else
                {
                    var followers = await _followService.GetFollowers(userId);
                    Assert.Empty(followers.Data!);
                }

            } else
            {
                var followers = await _followService.GetFollowers(9999);
                Assert.False(followers.Success);
            }

            await transaction.RollbackAsync();
        }



    }


}
