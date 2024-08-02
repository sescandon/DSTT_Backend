using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;

namespace DSTT_Backend.Repositories.IRepositories
{
    public interface IFollowRepository
    {
        Task<BasicOperationResult> FollowUser(int followerId, int followedId);

        Task<BasicOperationResult> UnFollowUser(Follow follow);

        Task<List<User>> GetFollowers(int userId);

        Task<List<User>> GetFollowing(int userId);

        Task<Follow?> IsFollowing(int followerId, int followedId);
    }
}
