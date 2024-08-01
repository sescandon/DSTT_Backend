using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DSTT_Test.RepositoriesTests
{
    public class MessageRepositoryTest
    {
        private readonly DsttDbContext _context;
        private readonly UserRepository _auxiliaryUserRepository;
        private readonly MessageRepository _messageRepository;

        public MessageRepositoryTest()
        {
            //string? testDb = Environment.GetEnvironmentVariable("TestDBConnectionString");
            string testDb = "Server=localhost;Database=DSTT_DB_TEST;Trusted_Connection=true;TrustServerCertificate=true";
            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _messageRepository = new MessageRepository(_context);
            _auxiliaryUserRepository = new UserRepository(_context);
        }

        [Fact]
        public async Task CreateMessage_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessageDTO { Content = "TestMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            Assert.Equal(message.Content, createdMessage.Content);
            Assert.Equal(message.UserId, createdMessage.UserId);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task DeleteMessage_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestDeleteMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessageDTO { Content = "TestDeleteMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            var result = await _messageRepository.DeleteMessage(createdMessage);

            Assert.True(result.Success);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task UpdateMessage_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestUpdateMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessageDTO { Content = "TestUpdateMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            var newContent = "UpdatedMessageContent";
            var result = await _messageRepository.UpdateMessage(newContent, createdMessage);

            Assert.True(result.Success);

            var updatedMessage = await _messageRepository.GetMessage(messageId);

            Assert.Equal(newContent, updatedMessage.Content);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task GetMessages_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestGetMessagesUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message1 = new MessageDTO { Content = "TestGetMessagesContent1", UserId = userId };
            var message2 = new MessageDTO { Content = "TestGetMessagesContent2", UserId = userId };

            await _messageRepository.CreateMessage(message1);
            await _messageRepository.CreateMessage(message2);

            var messages = await _messageRepository.GetMessages(userId);

            Assert.Equal(2, messages.Count);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task GetMessagesFromUserIds_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user1 = new UserDTO { Username = "TestGetMessagesFromUserIdsUser1" };
            var user2 = new UserDTO { Username = "TestGetMessagesFromUserIdsUser2" };
            var userId1 = await _auxiliaryUserRepository.CreateUser(user1);
            var userId2 = await _auxiliaryUserRepository.CreateUser(user2);

            var message1 = new MessageDTO { Content = "TestGetMessagesFromUserIdsContent1", UserId = userId1 };
            var message2 = new MessageDTO { Content = "TestGetMessagesFromUserIdsContent2", UserId = userId2 };

            await _messageRepository.CreateMessage(message1);
            await _messageRepository.CreateMessage(message2);

            var messages = await _messageRepository.GetMessagesFromUserIds(new List<int> { userId1, userId2 });

            Assert.Equal(2, messages.Count);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task GetMessage_Success()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var user = new UserDTO { Username = "TestGetMessageUser" };
            var userId = await _auxiliaryUserRepository.CreateUser(user);

            var message = new MessageDTO { Content = "TestGetMessageContent", UserId = userId };
            var messageId = await _messageRepository.CreateMessage(message);

            var createdMessage = await _messageRepository.GetMessage(messageId);

            Assert.NotNull(createdMessage);
            Assert.Equal(message.Content, createdMessage.Content);
            Assert.Equal(message.UserId, createdMessage.UserId);

            await transaction.RollbackAsync();
        }

        [Fact]
        public async Task GetMessage_MessageDoesNotExist_ReturnsNull()
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            var message = await _messageRepository.GetMessage(-1);

            Assert.Null(message);

            await transaction.RollbackAsync();
        }

    }
}
