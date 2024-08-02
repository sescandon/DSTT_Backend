using DSTT_Backend.Controllers;
using DSTT_Backend.Database;
using DSTT_Backend.Models.User;
using DSTT_Backend.Repositories;
using DSTT_Backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace DSTT_Test.ControllersTests
{
    public class UserControllerTest
    {
        private readonly DsttDbContext _context;
        private readonly UserRepository _userRepository;
        private readonly UserService _userService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            string testDb = Secret.TestDBConnectionString;
            var options = new DbContextOptionsBuilder<DsttDbContext>()
                .UseSqlServer(testDb)
                .Options;
            _context = new DsttDbContext(options);
            _userRepository = new UserRepository(_context);
            _userService = new UserService(_userRepository);
            _userController = new UserController(_userService);
        }

        private object? GetNestedPropertyValue(object obj, params string[] propertyNames)
        {
            object value = obj;
            foreach (var prop in propertyNames)
            {
                var property = value.GetType().GetProperty(prop);
                if (property == null)
                    return null;
                value = property.GetValue(value)!;
                if (value == null)
                    return null;
            }
            return value;
        }

        [Theory]
        [InlineData(true, true ,true)]
        [InlineData(true, false, true)]
        [InlineData(false, true ,false)]
        public async Task CreateUserAndGetUser_Test(bool userExists, bool id ,bool validData)
        {
            using IDbContextTransaction transaction = _context.Database.BeginTransaction();
            int userId = 9999;

            if(userExists)
            {
                if(validData)
                {
                    var newUser = new UserDTO { Username = "TestUser" };
                    var createdResult = await _userController.CreateUser(newUser);
                    var parsedCreatedResult = Assert.IsType<ObjectResult>(createdResult);
                    Assert.Equal(201, parsedCreatedResult.StatusCode);
                    userId = Convert.ToInt32(GetNestedPropertyValue(parsedCreatedResult.Value!, "Id"));
                    Assert.NotEqual(9999, userId);
                    
                }
                else
                {
                    var newUser = new UserDTO { Username = "" };
                    var createdResult = await _userController.CreateUser(newUser);
                    var parsedCreatedResult = Assert.IsType<ObjectResult>(createdResult);
                    Assert.Equal(400, parsedCreatedResult.StatusCode);
                }

                if(id)
                {
                    var getResult = await _userController.GetUserFromId(userId);
                    var parsedGetResult = Assert.IsType<ObjectResult>(getResult);
                    Assert.Equal(200, parsedGetResult.StatusCode);
                    var userName = GetNestedPropertyValue(parsedGetResult.Value!, "User", "Username") as string;
                    Assert.NotNull(userName);
                    Assert.Equal("TestUser", userName);
                }
                else
                {
                    var getResult = await _userController.GetUserFromUsername("TestUser");
                    var parsedGetResult = Assert.IsType<ObjectResult>(getResult);
                    Assert.Equal(200, parsedGetResult.StatusCode);
                    var userName = GetNestedPropertyValue(parsedGetResult.Value!, "User", "Username") as string;
                    Assert.NotNull(userName);
                    Assert.Equal("TestUser", userName);
                }
            }
            else
            {
                if(id)
                {
                    var getResult = await _userController.GetUserFromId(userId);
                    var parsedGetResult = Assert.IsType<ObjectResult>(getResult);
                    Assert.Equal(404, parsedGetResult.StatusCode);
                }
                else
                {
                    var getResult = await _userController.GetUserFromUsername("TestUser");
                    var parsedGetResult = Assert.IsType<ObjectResult>(getResult);
                    Assert.Equal(404, parsedGetResult.StatusCode);
                }
            }

            await transaction.RollbackAsync();
        }


    }
}
