using DSTT_Backend.Database;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;

namespace DSTT_Backend.Repositories.IRepositories
{
    public interface IMessageRepository
    {

        Task<Message?> GetMessage(int messageId);
        Task<List<Message>> GetMessages(int userId);

        Task<List<Message>> GetMessagesFromUserIds(List<int> userIds);

        Task<int> CreateMessage(MessagePostDTO message);

        Task<BasicOperationResult> DeleteMessage(Message message);

        Task<BasicOperationResult> UpdateMessage(string content, Message message);

    }
}
