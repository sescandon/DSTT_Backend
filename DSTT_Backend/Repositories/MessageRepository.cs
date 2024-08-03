using DSTT_Backend.Database;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DSTT_Backend.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DsttDbContext _context;

        public MessageRepository(DsttDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetMessages(int userId)
        {
            return await _context.Messages.Where(m => m.UserId == userId).OrderByDescending(message => message.CreatedDate).ToListAsync();
        }

        public async Task<Message?> GetMessage(int messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task<List<Message>> GetMessagesFromUserIds(List<int> userIds)
        {
            if (userIds.Count == 0 || userIds == null)
            {
                return new List<Message>();
            }

            var query = _context.Messages.Where(user => userIds.Contains(user.UserId))
                .OrderByDescending(message => message.CreatedDate);

            return await query.ToListAsync();
        }

        public async Task<int> CreateMessage(MessagePostDTO message)
        {
            var newMessage = new Message
            {
                Content = message.Content,
                UserId = message.UserId,
                CreatedDate = DateTime.Now
            };

            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            return newMessage.Id;
        }

        public async Task<BasicOperationResult> DeleteMessage(Message message)
        {
            try
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
                return new BasicOperationResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {

                return new BasicOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<BasicOperationResult> UpdateMessage(string content, Message message)
        {
            try
            {
                message.Content = content;
                await _context.SaveChangesAsync();
                return new BasicOperationResult
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new BasicOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
