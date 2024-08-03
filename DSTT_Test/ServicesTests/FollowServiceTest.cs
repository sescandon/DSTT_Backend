using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services;


namespace DSTT_Test.ServicesTests
{
    public class FollowServiceTest
    {
        
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IFollowRepository> _followRepository;
        private readonly FollowService _followService;

        public FollowServiceTest()
        {
            _userRepository = new Mock<IUserRepository>();
            _followRepository = new Mock<IFollowRepository>();
            _followService = new FollowService(_followRepository.Object, _userRepository.Object);
        }

        [Fact]
        public async Task FollowUser_FollowerOrFollowingDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            var result = await _followService.FollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("User with id 1 exists: False, User with id 2 exists: False", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task FollowUser_FollowerIsFolloweeFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());

            var result = await _followService.FollowUser(1, 1);

            Assert.False(result.Success);
            Assert.Equal("User can't follow itself", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Exactly(2));
        }

        [Fact]
        public async Task FollowUser_AlreadyFollowingFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Follow());

            var result = await _followService.FollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("Already following user", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);

        }

        [Fact]
        public async Task FollowUser_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _followService.FollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task FollowUser_Success()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Follow)null);

            var result = await _followService.FollowUser(1, 2);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);
        }

        [Fact]
        public async Task UnFollowUser_FollowerOrFollowingDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            var result = await _followService.UnFollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("User with id 1 exists: False, User with id 2 exists: False", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task UnFollowUser_FollowerIsFolloweeFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());

            var result = await _followService.UnFollowUser(1, 1);

            Assert.False(result.Success);
            Assert.Equal("User can't unfollow itself", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Exactly(2));
        }

        [Fact]
        public async Task UnFollowUser_NotFollowingFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync((Follow)null);

            var result = await _followService.UnFollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("Not following user", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);

        }

        [Fact]
        public async Task UnFollowUser_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _followService.UnFollowUser(1, 2);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task UnFollowUser_Success()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new Follow());

            var result = await _followService.UnFollowUser(1, 2);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);
            _followRepository.Verify(repo => repo.UnFollowUser(It.IsAny<Follow>()), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_UserDoesntExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            var result = await _followService.GetFollowers(1);

            Assert.False(result.Success);
            Assert.Equal("User doesn't exist", result.ErrorMessage);
            Assert.Equal(404, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _followService.GetFollowers(1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_EmptyListSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.GetFollowers(1)).ReturnsAsync(new List<User>());

            var result = await _followService.GetFollowers(1);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            Assert.Empty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowers(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_Success()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.GetFollowers(1)).ReturnsAsync(new List<User> { new User() });

            var result = await _followService.GetFollowers(1);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            Assert.NotEmpty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowers(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_UserDoesntExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            var result = await _followService.GetFollowing(1);

            Assert.False(result.Success);
            Assert.Equal("User doesn't exist", result.ErrorMessage);
            Assert.Equal(404, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _followService.GetFollowing(1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_EmptyListSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.GetFollowing(1)).ReturnsAsync(new List<User>());

            var result = await _followService.GetFollowing(1);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            Assert.Empty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowing(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_Success()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.GetFollowing(1)).ReturnsAsync(new List<User> { new User() });

            var result = await _followService.GetFollowing(1);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.Equal(200, result.StatusCode);
            Assert.NotEmpty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowing(1), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_FollowerOrFollowingDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync((User)null);

            var result = await _followService.IsFollowing(1, 2);

            Assert.False(result.Success);
            Assert.Equal("User with id 1 exists: False, User with id 2 exists: False", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_FollowerIsFolloweeFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());

            var result = await _followService.IsFollowing(1, 1);

            Assert.False(result.Success);
            Assert.Equal("User can't follow itself", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Exactly(2));
        }

        [Fact]
        public async Task IsFollowing_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _followService.IsFollowing(1, 2);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_NotFollowingSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(1, 2)).ReturnsAsync((Follow)null);

            var result = await _followService.IsFollowing(1, 2);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.False(result.Result);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_FollowingSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User());
            _userRepository.Setup(repo => repo.GetUserById(2)).ReturnsAsync(new User());
            _followRepository.Setup(repo => repo.IsFollowing(1, 2)).ReturnsAsync(new Follow());

            var result = await _followService.IsFollowing(1, 2);

            Assert.True(result.Success);
            Assert.Null(result.ErrorMessage);
            Assert.True(result.Result);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _userRepository.Verify(repo => repo.GetUserById(2), Times.Once);
            _followRepository.Verify(repo => repo.IsFollowing(1, 2), Times.Once);
        }

    }


}
