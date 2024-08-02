using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Repositories.IRepositories;
using DSTT_Backend.Services.IServices;

namespace DSTT_Backend.Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;

        public FollowService(IFollowRepository followRepository, IUserRepository userRepository)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceOperationResult> FollowUser(int followerId, int followeeId)
        {
            try
            {
                
                User? follower = await _userRepository.GetUserById(followerId);
                User? followee = await _userRepository.GetUserById(followeeId);

                bool followerExists = follower != null;
                bool followeeExists = followee != null;

                if (!followerExists || !followeeExists)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = $"User with id {followerId} exists: {followerExists}, User with id {followeeId} exists:{followeeExists}",
                        StatusCode = 400
                    };
                }

                if (followerId == followeeId)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User can't follow itself",
                        StatusCode = 400
                    };
                }

                Follow? follow = await _followRepository.IsFollowing(followerId, followeeId);


                if (follow != null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Already following user",
                        StatusCode = 400
                    };
                }

                await _followRepository.FollowUser(followerId, followeeId);

                return new ServiceOperationResult
                {
                    Success = true,
                    ErrorMessage = null,
                    StatusCode = 200
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

        public async Task<ServiceOperationResult> UnFollowUser(int followerId, int followeeId)
        {
            try
            {
                
                User? follower = await _userRepository.GetUserById(followerId);
                User? followee = await _userRepository.GetUserById(followeeId);

                bool followerExists = follower != null;
                bool followeeExists = followee != null;

                if (!followerExists || !followeeExists)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = $"User with id {followerId} exists: {followerExists}, User with id {followeeId} exists:{followeeExists}",
                        StatusCode = 400
                    };
                }

                if (followerId == followeeId)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User can't unfollow itself",
                        StatusCode = 400
                    };
                }

                Follow? follow = await _followRepository.IsFollowing(followerId, followeeId);

                if (follow == null)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "Not following user",
                        StatusCode = 400
                    };
                }

                await _followRepository.UnFollowUser(follow);

                return new ServiceOperationResult
                {
                    Success = true,
                    ErrorMessage = null,
                    StatusCode = 200
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

        public async Task<ServiceDataOperationResult<User>> GetFollowers(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ServiceDataOperationResult<User>
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                List<User> followers = await _followRepository.GetFollowers(userId);

                return new ServiceDataOperationResult<User>
                {
                    Success = true,
                    Data = followers,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<User>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceDataOperationResult<User>> GetFollowing(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ServiceDataOperationResult<User>
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                List<User> following = await _followRepository.GetFollowing(userId);

                return new ServiceDataOperationResult<User>
                {
                    Success = true,
                    Data = following,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<User>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceOperationResult> IsFollowing(int followerId, int followeeId)
        {
            try
            {
                
                User? follower = await _userRepository.GetUserById(followerId);
                User? followee = await _userRepository.GetUserById(followeeId);

                bool followerExists = follower != null;
                bool followeeExists = followee != null;

                if (!followerExists || !followeeExists)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = $"User with id {followerId} exists: {followerExists}, User with id {followeeId} exists:{followeeExists}",
                        StatusCode = 400
                    };
                }

                if (followerId == followeeId)
                {
                    return new ServiceOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User can't follow itself",
                        StatusCode = 400
                    };
                }

                Follow? follow = await _followRepository.IsFollowing(followerId, followeeId);

                return new ServiceOperationResult
                {
                    Success = follow != null
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
    }
}
