using DSTT_Backend.Database;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
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
        private readonly MessageService _messageService;
        private readonly DsttDbContext _context;
        private readonly MessageRepository _messageRepository;
        private readonly UserRepository _userRepository;
        private readonly FollowRepository _followRepository;

        public MessageServiceTest()
        {
            string testDb = Secret.TestDBConnectionString;
            var options = new DbContextOptionsBuilder<DsttDbContext>()
            .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _messageRepository = new MessageRepository(_context);
            _userRepository = new UserRepository(_context);
            _followRepository = new FollowRepository(_context);
            _messageService = new MessageService(_messageRepository, _userRepository, _followRepository);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task GetDashboardMessages_Test(bool userExists)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (userExists)
            {
                var followerUser = new UserDTO
                {
                    Username = "FollowerUser"
                };
                var createdFollowerResult = await _userRepository.CreateUser(followerUser);
                Assert.True(createdFollowerResult > 0);

                var followedUser = new UserDTO
                {
                    Username = "FollowerUser"
                };
                var createdFollowedResult = await _userRepository.CreateUser(followedUser);
                Assert.True(createdFollowedResult > 0);

                var followResult = await _followRepository.FollowUser(createdFollowerResult, createdFollowedResult);
                Assert.True(followResult.Success);

                var message = new MessagePostDTO
                {
                    UserId = createdFollowedResult,
                    Content = "Test message"
                };

                var messageResult = await _messageService.CreateMessage(message);
                Assert.True(messageResult.Id > 0);

                var dashboardMessages = await _messageService.GetDashboardMessages(createdFollowerResult);
                Assert.NotEmpty(dashboardMessages.Data!);
                Assert.True(dashboardMessages.Data!.Count == 1);
                Assert.Equal("Test message", dashboardMessages.Data![0].Content);
            }
            else
            {
                var dashboardMessages = await _messageService.GetDashboardMessages(9999);
                Assert.False(dashboardMessages.Success);
            }

            await transaction.RollbackAsync();
        }

        [Theory]
        [InlineData (true, true)]
        [InlineData(false, false)]
        public async Task GetUserMessages_Test(bool userExists, bool hasMessages)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (userExists)
            {
                var user = new UserDTO
                {
                    Username = "User"
                };
                var createdResult = await _userRepository.CreateUser(user);
                Assert.True(createdResult > 0);

                if (hasMessages)
                {
                    var message = new MessagePostDTO
                    {
                        UserId = createdResult,
                        Content = "First Test message"
                    };
                    var messageResult = await _messageService.CreateMessage(message);
                    Assert.True(messageResult.Id > 0);

                    var secondMessage = new MessagePostDTO
                    {
                        UserId = createdResult,
                        Content = "Second message"
                    };
                    var secondMessageResult = await _messageService.CreateMessage(message);
                    Assert.True(messageResult.Id > 0);

                    var messages = await _messageService.GetUserMessages(createdResult);
                    Assert.NotEmpty(messages.Data!);
                    Assert.True(messages.Data!.Count == 2);
                    Assert.Equal("First Test message", messages.Data![0].Content);

                }
                else
                {
                    var messages = await _messageService.GetUserMessages(createdResult);
                    Assert.Empty(messages.Data!);
                }


            }

            await transaction.RollbackAsync();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task UpdateMessage_Test(bool messageExists)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (messageExists)
            {
                var user = new UserDTO
                {
                    Username = "User"
                };
                var createdResult = await _userRepository.CreateUser(user);
                Assert.True(createdResult > 0);

                var message = new MessagePostDTO
                {
                    UserId = createdResult,
                    Content = "Test message"
                };
                var messageResult = await _messageService.CreateMessage(message);
                Assert.True(messageResult.Id > 0);

                string newContent = "New content";

                var updateResult = await _messageService.UpdateMessage(newContent,messageResult.Id.Value);

                Assert.True(updateResult.Success);
            }
            else
            {

                string newContent = "New content";

                var updateResult = await _messageService.UpdateMessage(newContent, 9999);

                Assert.False(updateResult.Success);
            }

            await transaction.RollbackAsync();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteMessage_Test(bool messageExists)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (messageExists)
            {
                var user = new UserDTO
                {
                    Username = "User"
                };
                var createdResult = await _userRepository.CreateUser(user);
                Assert.True(createdResult > 0);

                var message = new MessagePostDTO
                {
                    UserId = createdResult,
                    Content = "Test message"
                };
                var messageResult = await _messageService.CreateMessage(message);
                Assert.True(messageResult.Id > 0);

                var messageToDelete = await _messageRepository.GetMessage(messageResult.Id.Value);
                var deleteResult = await _messageRepository.DeleteMessage(messageToDelete!);
                Assert.True(deleteResult.Success);

                var deletedMessage = await _messageRepository.GetMessage(messageResult.Id.Value);
                Assert.Null(deletedMessage);
            }
            else
            {

                var deletedMessage = await _messageRepository.DeleteMessage(new Message());
                Assert.False(deletedMessage.Success);
            }

            await transaction.RollbackAsync();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CreateMessage_Test(bool userExists)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();

            if (userExists)
            {
                var user = new UserDTO
                {
                    Username = "User"
                };
                var createdResult = await _userRepository.CreateUser(user);
                Assert.True(createdResult > 0);

                var message = new MessagePostDTO
                {
                    UserId = createdResult,
                    Content = "Test message"
                };
                var messageResult = await _messageService.CreateMessage(message);
                Assert.True(messageResult.Id > 0);
            }
            else
            {
                var message = new MessagePostDTO
                {
                    UserId = 9999,
                    Content = "Test message"
                };
                var messageResult = await _messageService.CreateMessage(message);
                Assert.False(messageResult.Success);
            }

            await transaction.RollbackAsync();
        }


    }
}
