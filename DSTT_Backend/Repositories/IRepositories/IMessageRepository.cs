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

        Task<int> CreateMessage(MessageDTO message);

        Task<RepositoryOperationResult> DeleteMessage(Message message);

        Task<RepositoryOperationResult> UpdateMessage(string content, Message message);

    }
}
