using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;

namespace DSTT_Backend.Services.IServices
{
    public interface IMessageService
    {
        Task<ServiceDataOperationResult<MessageModel>> GetDashboardMessages(int userId);

        Task<ServiceDataOperationResult<MessageModel>> GetUserMessages(int userId);

        Task<ServiceOperationResult> UpdateMessage(string content, int messageId);

        Task<ServiceOperationResult> DeleteMessage(int messageId);

        Task<ServiceIDOperationResult> CreateMessage(MessagePostDTO message);
    }
}
