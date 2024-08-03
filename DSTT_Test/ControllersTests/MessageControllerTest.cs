using DSTT_Backend.Controllers;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Services.IServices;
using DSTT_Test.Utils;
using Microsoft.AspNetCore.Mvc;


namespace DSTT_Test.ControllersTests
{
    public class MessageControllerTest
    {
        private readonly Mock<IMessageService> _messageService;
        private readonly MessageController _messageController;

        public MessageControllerTest()
        {
            _messageService = new Mock<IMessageService>();
            _messageController = new MessageController(_messageService.Object);
        }

        [Fact]
        public async Task CreateMessage_Success()
        {
            var message = new MessagePostDTO { UserId = 1, Content = "Test message" };
            _messageService.Setup(service => service.CreateMessage(message)).ReturnsAsync(new ServiceIDOperationResult { Success = true, Id = 1 });

            var result = await _messageController.CreateMessage(message);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messageId = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "MessageID") as int?;
            Assert.Equal(1, messageId);
            _messageService.Verify(service => service.CreateMessage(message), Times.Once);
        }

        [Fact]
        public async Task CreateMessage_DatabaseFail()
        {
            var message = new MessagePostDTO { UserId = 1, Content = "Test message" };
            _messageService.Setup(service => service.CreateMessage(message)).ReturnsAsync(new ServiceIDOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _messageController.CreateMessage(message);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
            _messageService.Verify(service => service.CreateMessage(message), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_Success()
        {
            _messageService.Setup(service => service.DeleteMessage(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _messageController.DeleteMessage(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var message = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Message deleted", message);
            _messageService.Verify(service => service.DeleteMessage(1), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_MessageDoesntExistFail()
        {
            _messageService.Setup(service => service.DeleteMessage(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Message not found", StatusCode = 404 });

            var result = await _messageController.DeleteMessage(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Message not found", error);
            _messageService.Verify(service => service.DeleteMessage(1), Times.Once);
        }

        [Fact]
        public async Task DeleteMessage_DatabaseFail()
        {
            _messageService.Setup(service => service.DeleteMessage(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _messageController.DeleteMessage(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
            _messageService.Verify(service => service.DeleteMessage(1), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_Success()
        {
            var message = new MessagePutDTO { Content = "Test message" };
            _messageService.Setup(service => service.UpdateMessage(message.Content, It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _messageController.UpdateMessage(1, message);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messageResult = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Message updated", messageResult);
            _messageService.Verify(service => service.UpdateMessage(message.Content, 1), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_MessageDoesntExistFail()
        {
            var message = new MessagePutDTO { Content = "Test message" };
            _messageService.Setup(service => service.UpdateMessage(message.Content, It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Message not found", StatusCode = 404 });

            var result = await _messageController.UpdateMessage(1, message);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Message not found", error);
            _messageService.Verify(service => service.UpdateMessage(message.Content, 1), Times.Once);
        }

        [Fact]
        public async Task UpdateMessage_DatabaseFail()
        {
            var message = new MessagePutDTO { Content = "Test message" };
            _messageService.Setup(service => service.UpdateMessage(message.Content, It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _messageController.UpdateMessage(1, message);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
            _messageService.Verify(service => service.UpdateMessage(message.Content, 1), Times.Once);
        }

        [Fact]
        public async Task GetTimelineMessages_Success()
        {
            _messageService.Setup(service => service.GetUserMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = true, Data = new List<MessageModel> { new MessageModel { Id = 1, UserId = 1, Content = "Test message", CreatedDate = DateTime.Now } } });

            var result = await _messageController.GetTimelineMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messages = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Messages") as List<MessageModel>;
            Assert.Single(messages);
            Assert.Equal("Test message", messages[0].Content);
            _messageService.Verify(service => service.GetUserMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetTimelineMessages_UserDoesntExistFail()
        {
            _messageService.Setup(service => service.GetUserMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = false, ErrorMessage = "User not found", StatusCode = 404 });

            var result = await _messageController.GetTimelineMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("User not found", error);
            _messageService.Verify(service => service.GetUserMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetTimelineMessages_DatabaseFail()
        {
            _messageService.Setup(service => service.GetUserMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _messageController.GetTimelineMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
            _messageService.Verify(service => service.GetUserMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetTimelineMessages_NoMessages()
        {
            _messageService.Setup(service => service.GetUserMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = true, Data = new List<MessageModel>() });

            var result = await _messageController.GetTimelineMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messages = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Messages") as List<MessageModel>;
            Assert.Empty(messages);
            _messageService.Verify(service => service.GetUserMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_Success()
        {
            _messageService.Setup(service => service.GetDashboardMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = true, Data = new List<MessageModel> { new MessageModel { Id = 1, UserId = 1, Content = "Test message", CreatedDate = DateTime.Now } } });

            var result = await _messageController.GetDashboardMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messages = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Messages") as List<MessageModel>;
            Assert.Single(messages);
            Assert.Equal("Test message", messages[0].Content);
            _messageService.Verify(service => service.GetDashboardMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_UserDoesntExistFail()
        {
            _messageService.Setup(service => service.GetDashboardMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = false, ErrorMessage = "User not found", StatusCode = 404 });

            var result = await _messageController.GetDashboardMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("User not found", error);
            _messageService.Verify(service => service.GetDashboardMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_DatabaseFail()
        {
            _messageService.Setup(service => service.GetDashboardMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _messageController.GetDashboardMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
            _messageService.Verify(service => service.GetDashboardMessages(1), Times.Once);
        }

        [Fact]
        public async Task GetDashboardMessages_NoMessages()
        {
            _messageService.Setup(service => service.GetDashboardMessages(It.IsAny<int>())).ReturnsAsync(new ServiceDataOperationResult<MessageModel> { Success = true, Data = new List<MessageModel>() });

            var result = await _messageController.GetDashboardMessages(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var messages = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Messages") as List<MessageModel>;
            Assert.Empty(messages);
            _messageService.Verify(service => service.GetDashboardMessages(1), Times.Once);
        }


    }
}
