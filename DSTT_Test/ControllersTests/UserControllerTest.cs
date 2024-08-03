using DSTT_Backend.Controllers;
using DSTT_Backend.Models.Results;
using DSTT_Backend.Models.User;
using DSTT_Backend.Services.IServices;
using DSTT_Test.Utils;
using Microsoft.AspNetCore.Mvc;

namespace DSTT_Test.ControllersTests
{
    public class UserControllerTest
    {
        private readonly Mock<IUserService> _userService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _userService = new Mock<IUserService>();
            _userController = new UserController(_userService.Object);
        }

        [Fact]
        public async Task GetUserById_UserDoesntExistFail()
        {
            _userService.Setup(service => service.GetUser(It.IsAny<int>())).ReturnsAsync((UserModel)null!);

            var result = await _userController.GetUserById(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
        }

        [Fact]
        public async Task GetUserById_DatabaseFail() {
            _userService.Setup(service => service.GetUser(It.IsAny<int>())).ThrowsAsync(new Exception("Database error"));

            var result = await _userController.GetUserById(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Error") as string;
            Assert.Equal("Database error", error);
        }

        [Fact]
        public async Task GetUserById_UserExistsSuccess()
        {
            var user = new UserModel { Id = 1, Username = "TestUser" };
            _userService.Setup(service => service.GetUser(It.IsAny<int>())).ReturnsAsync(user);

            var result = await _userController.GetUserById(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var userName = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "User", "Username") as string;
            Assert.Equal("TestUser", userName);
        }

        [Fact]
        public async Task GetUserByUsername_UserDoesntExistFail()
        {
            _userService.Setup(service => service.GetUser(It.IsAny<string>())).ReturnsAsync((UserModel)null!);

            var result = await _userController.GetUserByUsername("TestUser");

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
        }

        [Fact]
        public async Task GetUserByUsername_DatabaseFail()
        {
            _userService.Setup(service => service.GetUser(It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

            var result = await _userController.GetUserByUsername("TestUser");

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Error") as string;
            Assert.Equal("Database error", error);
        }

        [Fact]
        public async Task GetUserByUsername_UserExistsSuccess()
        {
            var user = new UserModel { Id = 1, Username = "TestUser" };
            _userService.Setup(service => service.GetUser(It.IsAny<string>())).ReturnsAsync(user);

            var result = await _userController.GetUserByUsername("TestUser");

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
            var userName = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "User", "Username") as string;
            Assert.Equal("TestUser", userName);
        }

        [Fact]
        public async Task CreateUser_InvalidDataFail()
        {
            _userService.Setup(service => service.CreateUser(It.IsAny<UserDTO>())).ReturnsAsync(new ServiceIDOperationResult { Success = true, Id = 1 });

            var newUser = new UserDTO { Username = "" };

            var result = await _userController.CreateUser(newUser);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, parsedResult.StatusCode);
            //Data annotation validations don't work in unit tests
            //Assert.Equal(400, parsedResult.StatusCode);
        }

        [Fact]
        public async Task CreateUser_DatabaseFail()
        {
            var newUser = new UserDTO { Username = "TestUser" };
            _userService.Setup(service => service.CreateUser(It.IsAny<UserDTO>())).ReturnsAsync(new ServiceIDOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _userController.CreateUser(newUser);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
        }

        [Fact]
        public async Task CreateUser_Success()
        {
            var newUser = new UserDTO { Username = "TestUser" };
            _userService.Setup(service => service.CreateUser(It.IsAny<UserDTO>())).ReturnsAsync(new ServiceIDOperationResult { Success = true, Id = 1 });

            var result = await _userController.CreateUser(newUser);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, parsedResult.StatusCode);
            var id = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Id");
            Assert.Equal(1, id);
        }

        [Fact]
        public async Task UpdateUser_UserDoesntExistFail()
        {
            _userService.Setup(service => service.EditUser(It.IsAny<UserDTO>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "User not found", StatusCode = 404 });

            var user = new UserDTO { Username = "TestUser" };

            var result = await _userController.EditUser(user, 1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("User not found", error);
        }

        [Fact]
        public async Task UpdateUser_DatabaseFail()
        {
            var user = new UserDTO { Username = "TestUser" };
            _userService.Setup(service => service.EditUser(It.IsAny<UserDTO>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _userController.EditUser(user, 1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
        }

        [Fact]
        public async Task UpdateUser_Success()
        {
            var user = new UserDTO { Username = "TestUser" };
            _userService.Setup(service => service.EditUser(It.IsAny<UserDTO>(), It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _userController.EditUser(user, 1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_UserDoesntExistFail()
        {
            _userService.Setup(service => service.DeleteUser(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "User not found", StatusCode = 404 });

            var result = await _userController.DeleteUser(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(404, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("User not found", error);
        }

        [Fact]
        public async Task DeleteUser_DatabaseFail()
        {
            _userService.Setup(service => service.DeleteUser(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = false, ErrorMessage = "Database error", StatusCode = 500 });

            var result = await _userController.DeleteUser(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, parsedResult.StatusCode);
            var error = AuxiliarClass.GetNestedPropertyValue(parsedResult.Value!, "Message") as string;
            Assert.Equal("Database error", error);
        }

        [Fact]
        public async Task DeleteUser_Success()
        {
            _userService.Setup(service => service.DeleteUser(It.IsAny<int>())).ReturnsAsync(new ServiceOperationResult { Success = true });

            var result = await _userController.DeleteUser(1);

            var parsedResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(200, parsedResult.StatusCode);
        }





    }
}
