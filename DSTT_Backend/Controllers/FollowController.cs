using DSTT_Backend.Services.IServices;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Models.Follow;
using Microsoft.AspNetCore.Mvc;

namespace DSTT_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowController : Controller
    {
        private readonly IFollowService _followService;

        public FollowController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost]
        [Route("FollowUser")]
        public async Task<IActionResult> FollowUser([FromBody] FollowDTO followInfo)
        {
            ServiceOperationResult result = await _followService.FollowUser(followInfo.FollowerId, followInfo.FollowedId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = $"User with Id {followInfo.FollowerId} followed user with Id {followInfo.FollowedId}" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpPost]
        [Route("UnfollowUser")]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowDTO followInfo)
        {
            ServiceOperationResult result = await _followService.UnFollowUser(followInfo.FollowerId, followInfo.FollowedId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = $"User with Id {followInfo.FollowerId} stoppped following user with Id {followInfo.FollowedId}" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpGet]
        [Route("GetFollowers/{userId:int}")]
        public async Task<IActionResult> GetFollowers(int userId)
        {
            ServiceDataOperationResult<UserModel> followersResult = await _followService.GetFollowers(userId);
            if(followersResult.Success == false)
            {
                return StatusCode(followersResult.StatusCode!.Value, new { Message = followersResult.ErrorMessage });
            }

            List<UserModel> followers = followersResult.Data!;
            
            return StatusCode(200, new { Followers = followers });
        }

        [HttpGet]
        [Route("GetFollowing/{userId:int}")]
        public async Task<IActionResult> GetFollowing(int userId)
        {
            ServiceDataOperationResult<UserModel> followingResult = await _followService.GetFollowing(userId);
            if(followingResult.Success == false)
            {
                return StatusCode(followingResult.StatusCode!.Value, new { Message = followingResult.ErrorMessage });
            }

            List<UserModel> following = followingResult.Data!;
            
            return StatusCode(200, new { Following = following });
        }

        [HttpPost]
        [Route("IsFollowing")]
        public async Task<IActionResult> IsFollowing([FromBody] FollowDTO followInfo)
        {
            ServiceBooleanOperationResult result = await _followService.IsFollowing(followInfo.FollowerId, followInfo.FollowedId);
            if (result.Success)
            {
                return StatusCode(200, new { result.Result });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }
    }
}
