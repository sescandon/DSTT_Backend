using DSTT_Backend.Database;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace DSTT_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("GetUserById/{userId:int}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                UserModel? user = await _userService.GetUser(userId);
                if (user == null)
                {
                    return StatusCode(404, new { Message = "User not found" });
                }
                else
                {
                    return StatusCode(200, new { User = user });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetUserByUsername/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            try
            {
                UserModel? user = await _userService.GetUser(username);
                if (user == null)
                {
                    return StatusCode(404, new { Message = "User not found" });
                }
                else
                {
                    return StatusCode(200, new { User = user });
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost]
        [Route("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserDTO user)
        {
            ServiceIDOperationResult result = await _userService.CreateUser(user);
            if (result.Success)
            {
                return StatusCode(201, new { Message = "User created", Id = result.Id });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpPut]
        [Route("EditUser/{userId:int}")]
        public async Task<IActionResult> EditUser([FromBody] UserDTO updatedUser, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ServiceOperationResult result = await _userService.EditUser(updatedUser, userId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = "User updated" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }

        [HttpDelete]
        [Route("DeleteUser/{userId:int}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            ServiceOperationResult result = await _userService.DeleteUser(userId);
            if (result.Success)
            {
                return StatusCode(200, new { Message = "User deleted" });
            }
            else
            {
                return StatusCode(result.StatusCode!.Value, new { Message = result.ErrorMessage });
            }
        }
    }
}
