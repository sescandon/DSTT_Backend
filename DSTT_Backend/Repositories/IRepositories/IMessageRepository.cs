using DSTT_Backend.Database;
using DSTT_Backend.Models;
using DSTT_Backend.Models.Message;

namespace DSTT_Backend.Repositories.IRepositories
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessages(int userId);

        Task<List<Message>> GetMessagesFromUserIds(List<int> userIds);

        Task<int> CreateMessage(MessageDTO message);

        Task<OperationResult> DeleteMessage(Message message);

        Task<OperationResult> UpdateMessage(string content, Message message);

    }
}
