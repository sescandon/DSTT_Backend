using DSTT_Backend.Database;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTT_Test.ServicesTests
{
    public class MessageServiceTest
    {
        private readonly Mock<IMessageRepository> _messageRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IFollowRepository> _followRepository;
        private readonly MessageService _messageService;

        public MessageServiceTest() : base()
        {
            _messageRepository = new Mock<IMessageRepository>();
            _userRepository = new Mock<IUserRepository>();
            _followRepository = new Mock<IFollowRepository>();
            _messageService = new MessageService(_messageRepository.Object, _userRepository.Object, _followRepository.Object);
        }

        [Fact]
        public async Task GetDashboardMessages_UserDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null);

            var result = await _messageService.GetDashboardMessages(1);

            Assert.False(result.Success);
            Assert.Equal("User does not exist", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(x => x.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var result = await _messageService.GetDashboardMessages(1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_NoFollowsEmptyListSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _followRepository.Setup(repo => repo.GetFollowing(1)).ReturnsAsync(new List<User>());
            _messageRepository.Setup(repo => repo.GetMessagesFromUserIds(new List<int> { 1 })).ReturnsAsync(new List<Message>());

            var result = await _messageService.GetDashboardMessages(1);
            Assert.True(result.Success);
            Assert.Empty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowing(1), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessagesFromUserIds(new List<int> { 1 }), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_FollowsExistSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _followRepository.Setup(repo => repo.GetFollowing(It.IsAny<int>())).ReturnsAsync(new List<User> { new User { Id = 2, Username = "TestUser2" } });
            _messageRepository.Setup(repo => repo.GetMessagesFromUserIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Message> { new Message { Id = 1, UserId = 2, Content = "TestMessage", CreatedDate = DateTime.Now } });

            var result = await _messageService.GetDashboardMessages(1);
            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal(2, result.Data[0].UserId);
            Assert.Equal("TestMessage", result.Data[0].Content);
            _userRepository.Verify(repo => repo.GetUserById(It.IsAny<int>()), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowing(It.IsAny<int>()), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessagesFromUserIds(It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_FollowsAndPostsExistSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _followRepository.Setup(repo => repo.GetFollowing(It.IsAny<int>())).ReturnsAsync(new List<User> { new User { Id = 2, Username = "TestUser2" } });
            _messageRepository.Setup(repo => repo.GetMessagesFromUserIds(It.IsAny<List<int>>())).ReturnsAsync(new List<Message> { new Message { Id = 1, UserId = 2, Content = "TestMessage", CreatedDate = DateTime.Now }, new Message { Id = 2, UserId = 1, Content = "TestSelfMessage", CreatedDate = DateTime.Now.AddMinutes(1) } });

            var result = await _messageService.GetDashboardMessages(1);
            Assert.True(result.Success);
            Assert.Equal(2, result.Data!.Count);
            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal(2, result.Data[0].UserId);
            Assert.Equal(2, result.Data[1].Id);
            Assert.Equal(1, result.Data[1].UserId);
            Assert.Equal("TestMessage", result.Data[0].Content);
            _userRepository.Verify(repo => repo.GetUserById(It.IsAny<int>()), Times.Once);
            _followRepository.Verify(repo => repo.GetFollowing(It.IsAny<int>()), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessagesFromUserIds(It.IsAny<List<int>>()), Times.Once);
        }

        [Fact]
        public async Task GetUserMessages_UserDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null);

            var result = await _messageService.GetUserMessages(1);

            Assert.False(result.Success);
            Assert.Equal("User does not exist", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(x => x.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetUserMessages_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ThrowsAsync(new Exception("Database error"));

            var result = await _messageService.GetUserMessages(1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task GetUserMessages_NoMessagesEmptyListSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _messageRepository.Setup(repo => repo.GetMessages(1)).ReturnsAsync(new List<Message>());

            var result = await _messageService.GetUserMessages(1);
            Assert.True(result.Success);
            Assert.Empty(result.Data);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetUserMessages_MessagesExistSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _messageRepository.Setup(repo => repo.GetMessages(1)).ReturnsAsync(new List<Message> { new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now } });

            var result = await _messageService.GetUserMessages(1);
            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal(1, result.Data[0].UserId);
            Assert.Equal("TestMessage", result.Data[0].Content);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetUserMessages_MultipleMessagesExistSuccess()
        {
            _userRepository.Setup(repo => repo.GetUserById(It.IsAny<int>())).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _messageRepository.Setup(repo => repo.GetMessages(1)).ReturnsAsync(new List<Message> { new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now }, new Message { Id = 2, UserId = 1, Content = "TestMessage2", CreatedDate = DateTime.Now.AddMinutes(1) } });

            var result = await _messageService.GetUserMessages(1);
            Assert.True(result.Success);
            Assert.Equal(2, result.Data!.Count);
            Assert.Equal(1, result.Data[0].Id);
            Assert.Equal(1, result.Data[0].UserId);
            Assert.Equal(2, result.Data[1].Id);
            Assert.Equal(1, result.Data[1].UserId);
            Assert.Equal("TestMessage", result.Data[0].Content);
            _userRepository.Verify(repo => repo.GetUserById(It.IsAny<int>()), Times.Once);
            _messageRepository.Verify(repo => repo.GetMessages(1), Times.Once);
        }

        [Fact]
        public async Task CreateMessage_UserDoesNotExistFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync((User)null);

            var result = await _messageService.CreateMessage(new MessagePostDTO { UserId = 1, Content = "TestMessage" });

            Assert.False(result.Success);
            Assert.Equal("User does not exist", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _userRepository.Verify(x => x.GetUserById(1), Times.Once);
        }

        [Fact]
        public async Task CreateMessage_DatabaseFail()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _messageRepository.Setup(repo => repo.CreateMessage(It.IsAny<MessagePostDTO>())).ThrowsAsync(new Exception("Database error"));

            var result = await _messageService.CreateMessage(new MessagePostDTO { UserId = 1, Content = "TestMessage" });

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _messageRepository.Verify(repo => repo.CreateMessage(It.IsAny<MessagePostDTO>()), Times.Once);
        }

        [Fact]
        public async Task CreateMessage_Success()
        {
            _userRepository.Setup(repo => repo.GetUserById(1)).ReturnsAsync(new User { Id = 1, Username = "TestUser" });
            _messageRepository.Setup(repo => repo.CreateMessage(It.IsAny<MessagePostDTO>())).ReturnsAsync(1);

            var result = await _messageService.CreateMessage(new MessagePostDTO { UserId = 1, Content = "TestMessage" });

            Assert.True(result.Success);
            Assert.Equal(1, result.Id);
            _userRepository.Verify(repo => repo.GetUserById(1), Times.Once);
            _messageRepository.Verify(repo => repo.CreateMessage(It.IsAny<MessagePostDTO>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_MessageDoesNotExistFail()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync((Message)null);

            var result = await _messageService.DeleteMessage(1);

            Assert.False(result.Success);
            Assert.Equal("Message does not exist", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _messageRepository.Verify(x => x.GetMessage(1), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_DatabaseFail()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync(new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now });
            _messageRepository.Setup(repo => repo.DeleteMessage(It.IsAny<Message>())).ThrowsAsync(new Exception("Database error"));

            var result = await _messageService.DeleteMessage(1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _messageRepository.Verify(repo => repo.GetMessage(1), Times.Once);
            _messageRepository.Verify(repo => repo.DeleteMessage(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_Success()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync(new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now });
            _messageRepository.Setup(repo => repo.DeleteMessage(It.IsAny<Message>())).ReturnsAsync(new BasicOperationResult { Success = true });

            var result = await _messageService.DeleteMessage(1);

            Assert.True(result.Success);
            _messageRepository.Verify(repo => repo.GetMessage(1), Times.Once);
            _messageRepository.Verify(repo => repo.DeleteMessage(It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_MessageDoesNotExistFail()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync((Message)null);

            var result = await _messageService.UpdateMessage("TestMessage", 1);

            Assert.False(result.Success);
            Assert.Equal("Message does not exist", result.ErrorMessage);
            Assert.Equal(400, result.StatusCode);
            _messageRepository.Verify(x => x.GetMessage(1), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_DatabaseFail()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync(new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now });
            _messageRepository.Setup(repo => repo.UpdateMessage(It.IsAny<string>(), It.IsAny<Message>())).ThrowsAsync(new Exception("Database error"));

            var result = await _messageService.UpdateMessage("TestMessage", 1);

            Assert.False(result.Success);
            Assert.Equal("Database error", result.ErrorMessage);
            Assert.Equal(500, result.StatusCode);
            _messageRepository.Verify(repo => repo.GetMessage(1), Times.Once);
            _messageRepository.Verify(repo => repo.UpdateMessage(It.IsAny<string>(), It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_Success()
        {
            _messageRepository.Setup(repo => repo.GetMessage(1)).ReturnsAsync(new Message { Id = 1, UserId = 1, Content = "TestMessage", CreatedDate = DateTime.Now });
            _messageRepository.Setup(repo => repo.UpdateMessage(It.IsAny<string>(), It.IsAny<Message>())).ReturnsAsync(new BasicOperationResult { Success = true });

            var result = await _messageService.UpdateMessage("TestMessage", 1);

            Assert.True(result.Success);
            _messageRepository.Verify(repo => repo.GetMessage(1), Times.Once);
            _messageRepository.Verify(repo => repo.UpdateMessage(It.IsAny<string>(), It.IsAny<Message>()), Times.Once);
        }


    }
}
