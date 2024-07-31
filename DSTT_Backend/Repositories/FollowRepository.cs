using DSTT_Backend.Database;
using DSTT_Backend.Models;
using DSTT_Backend.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace DSTT_Backend.Repositories
{
    public class FollowRepository : IFollowRepository
    {
        private readonly DsttDbContext _dbcontext;

        public FollowRepository(DsttDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<Follow?> IsFollowing(int followerId, int followedId)
        {
            Follow? follow = await _dbcontext.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
            return follow; ;
        }

        public async Task<OperationResult> FollowUser(int followerId, int followedId)
        {
            try
            {
                Follow follow = new()
                {
                    FollowerId = followerId,
                    FollowedId = followedId
                };

                await _dbcontext.Follows.AddAsync(follow);
                await _dbcontext.SaveChangesAsync();
                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<OperationResult> UnFollowUser(Follow follow)
        {
            try
            {
                _dbcontext.Follows.Remove(follow);
                await _dbcontext.SaveChangesAsync();
                return new OperationResult { Success = true };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<List<User>> GetFollowers(int userId)
        {
            List<User> followers = await _dbcontext.Follows
                .Where(f => f.FollowedId == userId)
                .Select(f => f.Follower)
                .ToListAsync();

            return followers;
        }

        public async Task<List<User>> GetFollowing(int userId)
        {
            List<User> following = await _dbcontext.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followed)
                .ToListAsync();

            return following;
        }
    }
}
