using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;

namespace DSTT_Backend.Services.IServices
{
    public interface IFollowService
    {

        Task<ServiceOperationResult> FollowUser(int followerId, int followeeId);


        Task<ServiceOperationResult> UnFollowUser(int followerId, int followeeId);

        Task<ServiceDataOperationResult<UserModel>> GetFollowers(int userId);

        Task<ServiceDataOperationResult<UserModel>> GetFollowing(int userId);

        Task<ServiceBooleanOperationResult> IsFollowing(int followerId, int followeeId);


    }
}
