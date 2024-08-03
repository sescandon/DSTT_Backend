using DSTT_Backend.Controllers;
using DSTT_Backend.Models.Follow;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Services.IServices;
using DSTT_Test.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DSTT_Test.ControllersTests
{
    public class FollowControllerTest
    {
        private readonly Mock<IFollowService> _followService;
        private readonly FollowController _followController;

        public FollowControllerTest()
        {
            _followService = new Mock<IFollowService>();
            _followController = new FollowController(_followService.Object);
        }

        [Fact]
        public async Task FollowUser_UserDoesntExistFail()
        {
            _followService.Setup(service => service.FollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User with id 1 exists: False, User with id 2 exists: False" });

            var result = await _followController.FollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.FollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task FollowUser_UserFollowsItselfFail()
        {
            _followService.Setup(service => service.FollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User can't follow itself" });

            var result = await _followController.FollowUser(new FollowDTO { FollowerId = 1, FollowedId = 1 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.FollowUser(1, 1), Times.Once);
        }

        [Fact]
        public async Task FollowUser_AlreadyFollowingFail()
        {
            _followService.Setup(service => service.FollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "Already following user" });

            var result = await _followController.FollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.FollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task FollowUser_Success()
        {
            _followService.Setup(service => service.FollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _followController.FollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            _followService.Verify(service => service.FollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task UnfollowUser_UserDoesntExistFail()
        {
            _followService.Setup(service => service.UnFollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User with id 1 exists: False, User with id 2 exists: False" });

            var result = await _followController.UnfollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.UnFollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task UnfollowUser_UserUnfollowsItselfFail()
        {
            _followService.Setup(service => service.UnFollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User can't unfollow itself" });

            var result = await _followController.UnfollowUser(new FollowDTO { FollowerId = 1, FollowedId = 1 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.UnFollowUser(1, 1), Times.Once);
        }

        [Fact]
        public async Task UnfollowUser_NotFollowingFail()
        {
            _followService.Setup(service => service.UnFollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, StatusCode = 400, ErrorMessage = "Not following user" });

            var result = await _followController.UnfollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.UnFollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task UnfollowUser_Success()
        {
            _followService.Setup(service => service.UnFollowUser(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _followController.UnfollowUser(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            _followService.Verify(service => service.UnFollowUser(1, 2), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_UserDoesntExistFail()
        {
            _followService.Setup(service => service.GetFollowers(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = false, StatusCode = 400, ErrorMessage = "User not found" });

            var result = await _followController.GetFollowers(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.GetFollowers(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_EmptyListSuccess()
        {
            _followService.Setup(service => service.GetFollowers(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = true, Data = new List<UserModel>() });

            var result = await _followController.GetFollowers(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            _followService.Verify(service => service.GetFollowers(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowers_Success()
        {
            _followService.Setup(service => service.GetFollowers(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = true, Data = new List<UserModel> { new UserModel { Id = 1, Username = "TestUser" } } });

            var result = await _followController.GetFollowers(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var followers = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Followers") as List<UserModel>;
            Assert.Single(followers!);
            Assert.Equal(1, followers![0].Id);
            Assert.Equal("TestUser", followers![0].Username);
            _followService.Verify(service => service.GetFollowers(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_UserDoesntExistFail()
        {
            _followService.Setup(service => service.GetFollowing(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = false, StatusCode = 400, ErrorMessage = "User not found" });

            var result = await _followController.GetFollowing(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.GetFollowing(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_EmptyListSuccess()
        {
            _followService.Setup(service => service.GetFollowing(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = true, Data = new List<UserModel>() });

            var result = await _followController.GetFollowing(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            _followService.Verify(service => service.GetFollowing(1), Times.Once);
        }

        [Fact]
        public async Task GetFollowing_Success()
        {
            _followService.Setup(service => service.GetFollowing(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<UserModel> { Success = true, Data = new List<UserModel> { new UserModel { Id = 1, Username = "TestUser" } } });

            var result = await _followController.GetFollowing(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var following = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Following") as List<UserModel>;
            Assert.Single(following!);
            Assert.Equal(1, following![0].Id);
            Assert.Equal("TestUser", following![0].Username);
            _followService.Verify(service => service.GetFollowing(1), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_UsersDontExistFail()
        {
            _followService.Setup(service => service.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceBooleanOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User with id 1 exists: False, User with id 2 exists: False" });

            var result = await _followController.IsFollowing(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.IsFollowing(1, 2), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_UserFollowsItselfFail()
        {
            _followService.Setup(service => service.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceBooleanOperationResult { Success = false, StatusCode = 400, ErrorMessage = "User can't follow itself" });

            var result = await _followController.IsFollowing(new FollowDTO { FollowerId = 1, FollowedId = 1 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(400, parsedResult.StatusCode);
            _followService.Verify(service => service.IsFollowing(1, 1), Times.Once);
        }

        [Fact]
        public async Task IsFollowing_Success()
        {
            _followService.Setup(service => service.IsFollowing(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ServiceBooleanOperationResult { Success = true, Result = true });

            var result = await _followController.IsFollowing(new FollowDTO { FollowerId = 1, FollowedId = 2 });

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var isFollowing = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "IsFollowing") as bool?;
            Assert.True(isFollowing);
            _followService.Verify(service => service.IsFollowing(1, 2), Times.Once);
        }

    }
}
