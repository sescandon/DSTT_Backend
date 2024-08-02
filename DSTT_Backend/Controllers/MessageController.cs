using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DSTT_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        [Route("CreateMessage")]
        public async Task<IActionResult> CreateMessage([FromBody] MessagePostDTO message)
        {
            ServiceIDOperationResult result = await _messageService.CreateMessage(message);
            if (result.Success)
            {
                return StatusCode(200, new { MessageID = result.Id });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpDelete]
        [Route("DeleteMessage/{messageId:int}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            ServiceOperationResult result = await _messageService.DeleteMessage(messageId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = "Message deleted" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpPut]
        [Route("UpdateMessage/{messageId:int}")]
        public async Task<IActionResult> UpdateMessage(int messageId, [FromBody] MessagePutDTO Message)
        {
            ServiceOperationResult result = await _messageService.UpdateMessage(Message.Content, messageId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = "Message updated" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpGet]
        [Route("GetTimelineMessages/{userId:int}")]
        public async Task<IActionResult> GetTimelineMessages(int userId)
        {
            ServiceDataOperationResult<MessageModel> messagesResult = await _messageService.GetUserMessages(userId);
            if (messagesResult.Success == false)
            {
                return StatusCode(messagesResult.StatusCode!.Value, new { Message = messagesResult.ErrorMessage });
            }

            List<MessageModel> messages = messagesResult.Data!;

            return StatusCode(200, new { Messages = messages });
        }

        [HttpGet]
        [Route("GetDashboardMessages/{userId:int}")]
        public async Task<IActionResult> GetDashboardMessages(int userId)
        {
            ServiceDataOperationResult<MessageModel> messagesResult = await _messageService.GetDashboardMessages(userId);
            if (messagesResult.Success == false)
            {
                return StatusCode(messagesResult.StatusCode!.Value, new { Message = messagesResult.ErrorMessage });
            }

            List<MessageModel> messages = messagesResult.Data!;

            return StatusCode(200, new { Messages = messages });
        }

    }
}
