using DSTT_Backend.Database;
using DSTT_Backend.Models.Message;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services.IServices;

namespace DSTT_Backend.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFollowRepository _followRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IFollowRepository followRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _followRepository = followRepository;
        }

        public async Task<ServiceDataOperationResult<MessageModel>> GetDashboardMessages(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    return new ServiceDataOperationResult<MessageModel>
                    {
                        Success = false,
                        ErrorMessage = "User does not exist",
                        StatusCode = 400
                    };
                }

                List<User> followees = await _followRepository.GetFollowing(userId);
                List<int> followeeIds = followees.Select(user => user.Id).ToList();
                followeeIds.Add(userId);

                List<Message> rawMessages = await _messageRepository.GetMessagesFromUserIds(followeeIds);

                List<MessageModel> messageModels = rawMessages.Select(messages => new MessageModel
                {
                    Id = messages.Id,
                    UserId = messages.UserId,
                    Content = messages.Content
                }).ToList();

                return new ServiceDataOperationResult<MessageModel>
                {
                    Success = true,
                    Data = messageModels
                };


            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<MessageModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceDataOperationResult<MessageModel>> GetUserMessages(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);

                if (user == null)
                {
                    return new ServiceDataOperationResult<MessageModel>
                    {
                        Success = false,
                        ErrorMessage = "User does not exist",
                        StatusCode = 400
                    };
                }

                var messages = await _messageRepository.GetMessages(userId);
                List<MessageModel> messageModels = messages.Select(messages => new MessageModel
                {
                    Id = messages.Id,
                    UserId = messages.UserId,
                    Content = messages.Content
                }).ToList();

                return new ServiceDataOperationResult<MessageModel>
                {
                    Success = true,
                    Data = messageModels
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<MessageModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceOperationResult> UpdateMessage(string content, int messageId)
        {
            try
            {
                Message? message = await _messageRepository.GetMessage(messageId);

                if (message == null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Message does not exist",
                        StatusCode = 400
                    };
                }

                var result = await _messageRepository.UpdateMessage(content, message);

                return new ServiceOperationResult
                {
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage,
                };
            }
            catch (Exception ex)
            {

                return new ServiceOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceOperationResult> DeleteMessage(int messageId)
        {
            try
            {
                Message? message = await _messageRepository.GetMessage(messageId);

                if (message == null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Message does not exist",
                        StatusCode = 400
                    };
                }

                var result = await _messageRepository.DeleteMessage(message);



                return new ServiceOperationResult
                {
                    Success = result.Success,
                    ErrorMessage = result.ErrorMessage,
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<MessageModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceIDOperationResult> CreateMessage(MessageDTO message)
        {
            try
            {
                User? user = await _userRepository.GetUserById(message.UserId);

                if (user == null)
                {
                    return new ServiceIDOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User does not exist",
                        StatusCode = 400
                    };
                }

                int messageId = await _messageRepository.CreateMessage(message);

                return new ServiceIDOperationResult
                {
                    Success = true,
                    Id = messageId
                };
            }
            catch (Exception ex)
            {

                return new ServiceIDOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
