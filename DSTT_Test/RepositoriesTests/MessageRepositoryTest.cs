using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DSTT_Test.RepositoriesTests
{
    public class MessageRepositoryTest : BaseTest
    {
        private readonly UserRepository _auxiliaryUserRepository;
        private readonly MessageRepository _messageRepository;

        public MessageRepositoryTest() : base()
        {
            _messageRepository = new MessageRepository(_context);
            _auxiliaryUserRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateMessage_Success()
        {
            ClearDatabase();

            var user = new UserDTO { Username = "TestMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessagePostDTO { Content = "TestMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            Assert.Equal(message.Content, createdMessage!.Content);
            Assert.Equal(message.UserId, createdMessage.UserId);

            
        }

        [Fact]
        public async Task DeleteMessage_Success()
        {
            ClearDatabase();

            var user = new UserDTO { Username = "TestDeleteMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessagePostDTO { Content = "TestDeleteMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            var result = await _messageRepository.DeleteMessage(createdMessage!);

            Assert.True(result.Success);

            
        }

        [Fact]
        public async Task UpdateMessage_Success()
        {
            ClearDatabase();

            var user = new UserDTO { Username = "TestUpdateMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessagePostDTO { Content = "TestUpdateMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            var newContent = "UpdatedMessageContent";
            var result = await _messageRepository.UpdateMessage(newContent, createdMessage!);

            Assert.True(result.Success);

            var updatedMessage = await _messageRepository.GetMessage(messageId);

            Assert.Equal(newContent, updatedMessage!.Content);

            
        }

        [Fact]
        public async Task GetMessages_Success()
        {
            ClearDatabase();

            var user = new UserDTO { Username = "TestGetMessagesUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message1 = new MessagePostDTO { Content = "TestGetMessagesContent1", UserId = userId };
            var message2 = new MessagePostDTO { Content = "TestGetMessagesContent2", UserId = userId };

            await _messageRepository.CreateMessage(message1);
            await _messageRepository.CreateMessage(message2);

            var messages = await _messageRepository.GetMessages(userId);

            Assert.Equal(2, messages.Count);
            Assert.Equal(message1.Content, messages[1].Content);
            Assert.Equal(message2.Content, messages[0].Content);
            
        }

        [Fact]
        public async Task GetMessagesFromUserIds_Success()
        {
            ClearDatabase();

            var user1 = new UserDTO { Username = "TestGetMessagesFromUserIdsUser1" };
            var user2 = new UserDTO { Username = "TestGetMessagesFromUserIdsUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            var message1 = new MessagePostDTO { Content = "TestGetMessagesFromUserIdsContent1", UserId = userId1 };
            var message2 = new MessagePostDTO { Content = "TestGetMessagesFromUserIdsContent2", UserId = userId2 };

            await _messageRepository.CreateMessage(message1);
            await _messageRepository.CreateMessage(message2);

            var messages = await _messageRepository.GetMessagesFromUserIds(new List<int> { userId1, userId2 });

            Assert.Equal(2, messages.Count);
            Assert.Equal(message1.Content, messages[1].Content);
            Assert.Equal(message2.Content, messages[0].Content);

            
        }

        [Fact]
        public async Task GetMessage_Success()
        {
            ClearDatabase();

            var user = new UserDTO { Username = "TestGetMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessagePostDTO { Content = "TestGetMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            Assert.NotNull(createdMessage);
            Assert.Equal(message.Content, createdMessage.Content);
            Assert.Equal(message.UserId, createdMessage.UserId);

            
        }

        [Fact]
        public async Task GetMessage_MessageDoesNotExist_ReturnsNull()
        {
            ClearDatabase();

            var message = await _messageRepository.GetMessage(-1);

            Assert.Null(message);

            
        }

    }
}
