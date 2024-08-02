using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;

namespace DSTT_Backend.Services.IServices
{
    public interface IFollowService
    {

        Task<ServiceOperationResult> FollowUser(int followerId, int followeeId);


        Task<ServiceOperationResult> UnFollowUser(int followerId, int followeeId);

        Task<ServiceDataOperationResult<User>> GetFollowers(int userId);

        Task<ServiceDataOperationResult<User>> GetFollowing(int userId);

        Task<ServiceOperationResult> IsFollowing(int followerId, int followeeId);


    }
}
