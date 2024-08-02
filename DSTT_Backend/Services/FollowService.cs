using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
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
                        ErrorMessage = $"User with id {followerId} exists: {followerExists}, User with id {followeeId} exists: {followeeExists}",
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

        public async Task<ServiceDataOperationResult<UserModel>> GetFollowers(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ServiceDataOperationResult<UserModel>
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                List<User> followers = await _followRepository.GetFollowers(userId);

                List<UserModel> followersList = followers.Select(follower => new UserModel
                {
                    Id = follower.Id,
                    Username = follower.Username
                }).ToList();

                return new ServiceDataOperationResult<UserModel>
                {
                    Success = true,
                    Data = followersList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<UserModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceDataOperationResult<UserModel>> GetFollowing(int userId)
        {
            try
            {
                User? user = await _userRepository.GetUserById(userId);
                if (user == null)
                {
                    return new ServiceDataOperationResult<UserModel>
                    {
                        Success = false,
                        ErrorMessage = "User doesn't exist",
                        StatusCode = 404
                    };
                }

                List<User> followingResult = await _followRepository.GetFollowing(userId);

                List<UserModel> followingList = followingResult.Select(followee => new UserModel
                {
                    Id = followee.Id,
                    Username = followee.Username
                }).ToList();

                return new ServiceDataOperationResult<UserModel>
                {
                    Success = true,
                    Data = followingList,
                    StatusCode = 200
                };
            }
            catch (Exception ex)
            {

                return new ServiceDataOperationResult<UserModel>
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceBooleanOperationResult> IsFollowing(int followerId, int followeeId)
        {
            try
            {
                
                User? follower = await _userRepository.GetUserById(followerId);
                User? followee = await _userRepository.GetUserById(followeeId);

                bool followerExists = follower != null;
                bool followeeExists = followee != null;

                if (!followerExists || !followeeExists)
                {
                    return new ServiceBooleanOperationResult
                    {
                        Success = false,
                        ErrorMessage = $"User with id {followerId} exists: {followerExists}, User with id {followeeId} exists:{followeeExists}",
                        StatusCode = 400
                    };
                }

                if (followerId == followeeId)
                {
                    return new ServiceBooleanOperationResult
                    {
                        Success = false,
                        ErrorMessage = "User can't follow itself",
                        StatusCode = 400
                    };
                }

                Follow? follow = await _followRepository.IsFollowing(followerId, followeeId);

                return new ServiceBooleanOperationResult
                {
                    Success = true,
                    Result = follow != null,
                };
            }
            catch (Exception ex)
            {

                return new ServiceBooleanOperationResult
                {
                    Success = false,
                    ErrorMessage = ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
