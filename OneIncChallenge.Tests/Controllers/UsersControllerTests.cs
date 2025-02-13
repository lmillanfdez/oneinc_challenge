using Moq;
using Microsoft.AspNetCore.Mvc;
using OneIncChallenge.Controllers;
using OneIncChallenge.Exceptions;
using OneIncChallenge.Models.User;
using OneIncChallenge.Services.Contracts;
using OneIncChallenge.Tests.Helpers;
using Microsoft.AspNetCore.Http;

namespace OneIncChallenge.Tests.Controllers;

public class UsersControllerTests
{
    public UsersControllerTests()
    {
    }

    [Fact]
    public async Task GetUsers_Returns_OkResult()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        var returnedUsers = new List<UserDetailsDTO>() {};

        usersServiceMock.Setup(serv => serv.GetDetailedUsers())
                        .Returns(Task.FromResult(returnedUsers.AsEnumerable()));

        //act
        var usersController = new UsersController(usersServiceMock.Object);
        var users = await usersController.GetUsers();

        //assert
        Assert.IsType<OkObjectResult>(users);
    }

    [Fact]
    public async Task GetUsers_Returns_InternalServerErrorResponse()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        usersServiceMock
            .Setup(serv => serv.GetDetailedUsers())
            .ThrowsAsync(new Exception("Testing exception..."));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.GetUsers();

        //assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal((result as ObjectResult)?.StatusCode, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task GetUserById_Returns_BadRequest_WithEmptyUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.GetDetailedUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult<UserDetailsDTO?>(null));

        //act
        var usersController = new UsersController(usersServiceMock.Object);
        var users = await usersController.GetUserById(string.Empty);

        //assert
        Assert.IsType<BadRequestObjectResult>(users);
    }

    [Fact]
    public async Task GetUserById_Returns_BadRequest_WithInvalidUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.GetDetailedUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult<UserDetailsDTO?>(null));

        //act
        var usersController = new UsersController(usersServiceMock.Object);
        var users = await usersController.GetUserById("invalid-guid");

        //assert
        Assert.IsType<BadRequestObjectResult>(users);
    }

    [Fact]
    public async Task GetUserById_Returns_OkResult_WithValidUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.GetDetailedUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult<UserDetailsDTO?>(null));

        //act
        var usersController = new UsersController(usersServiceMock.Object);
        var users = await usersController.GetUserById("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");

        //assert
        Assert.IsType<OkObjectResult>(users);
    }

    [Fact]
    public async Task GetUserById_Returns_InternalServerErrorResponse()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.GetDetailedUser(It.IsAny<Guid>()))
                        .ThrowsAsync(new Exception("Testing exception..."));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.GetUserById("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");

        //assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal((result as ObjectResult)?.StatusCode, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task AddUser_Returns_CreatedResult_OnValidUser()
    {
        //arrage
        var usersServiceMock = new Mock<IUsersService>();
        var validUser = new User 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        usersServiceMock.Setup(us => us.AddUser(It.IsAny<User>()))
                        .ReturnsAsync(validUser);

        var usersController = new UsersController(usersServiceMock.Object);
    
        //act
        var result = await usersController.AddUser(validUser);
        var returnedObject = (result as CreatedResult)?.Value as User;
    
        //assert
        Assert.IsType<CreatedResult>(result);
        Assert.NotNull(returnedObject);
        Assert.Equal(SerializationHelper.SerializeResult(validUser), SerializationHelper.SerializeResult(returnedObject));
    }

    [Fact]
    public async Task AddUser_Returns_BadRequest_OnInvalidUser()
    {
        //arrage
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(us => us.AddUser(It.IsAny<User>()))
                        .ThrowsAsync(new InvalidUserException("invalid user..."));

        var usersController = new UsersController(usersServiceMock.Object);
        var invalidUser = new User 
        {
            FirstName = null,
            LastName = "Doe",
            Email = "john@com",
            DateOfBirth = DateTime.Now.AddYears(-15),
            PhoneNumber = 0
        };

        //act
        var result = await usersController.AddUser(invalidUser);
    
        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task AddUser_Returns_InternalServerErrorResponse_OnUnexpectedException()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(us => us.AddUser(It.IsAny<User>()))
                        .ThrowsAsync(new Exception("testing exception..."));

        var usersController = new UsersController(usersServiceMock.Object);
        var validUser = new User 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        //act
        var result = await usersController.AddUser(validUser);

        //assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal((result as ObjectResult)?.StatusCode, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_BadRequest_WithEmptyUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };
        
        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist(string.Empty, validUser);

        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_BadRequest_WithInvalidUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };
        
        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist("invalid-guid", validUser);

        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_CreatedResult_IfUserWasSuccessfullyCreated()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };
        
        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4", validUser);
        var returnedObject = (result as CreatedResult)?.Value as UserPutRequestDTO;
    
        //assert
        Assert.IsType<CreatedResult>(result);
        Assert.NotNull(returnedObject);
        Assert.Equal(SerializationHelper.SerializeResult(validUser), SerializationHelper.SerializeResult(returnedObject));
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_OkResult_IfUserWasSuccessfullyUpdated()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };
        
        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .Returns(Task.FromResult(false));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4", validUser);
    
        //assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_BadRequest_OnInvalidUser()
    {
        //arrage
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .ThrowsAsync(new InvalidUserException("invalid user..."));

        var usersController = new UsersController(usersServiceMock.Object);
        var invalidUser = new UserPutRequestDTO 
        {
            FirstName = null,
            LastName = "Doe",
            Email = "john@com",
            DateOfBirth = DateTime.Now.AddYears(-15),
            PhoneNumber = 0
        };

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4", invalidUser);
    
        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_InternalServerErrorResponse_OnUnexpectedException()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                        .ThrowsAsync(new Exception("testing exception..."));

        var usersController = new UsersController(usersServiceMock.Object);
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        //act
        var result = await usersController.UpdateUserOrCreateIfNotExist("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4", validUser);

        //assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal((result as ObjectResult)?.StatusCode, StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public async Task DeleteUser_Returns_BadRequest_WithEmptyUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        
        usersServiceMock.Setup(serv => serv.DeleteUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.DeleteUser(string.Empty);

        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_Returns_BadRequest_WithInvalidUserId()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        
        usersServiceMock.Setup(serv => serv.DeleteUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.DeleteUser("invalid-guid");

        //assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteUser_Returns_OkResult_IfUserWasSuccessfullyRemoved()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        
        usersServiceMock.Setup(serv => serv.DeleteUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult(true));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.DeleteUser("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");

        //assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteUser_Returns_NotFoundResult_IfUserWasNotRemoved()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();
        
        usersServiceMock.Setup(serv => serv.DeleteUser(It.IsAny<Guid>()))
                        .Returns(Task.FromResult(false));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.DeleteUser("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");

        //assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteUser_Returns_InternalServerErrorResponse_OnUnexpectedException()
    {
        //arrange
        var usersServiceMock = new Mock<IUsersService>();

        usersServiceMock.Setup(serv => serv.DeleteUser(It.IsAny<Guid>()))
                        .ThrowsAsync(new Exception("testing exception..."));

        var usersController = new UsersController(usersServiceMock.Object);

        //act
        var result = await usersController.DeleteUser("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");

        //assert
        Assert.IsType<ObjectResult>(result);
        Assert.Equal((result as ObjectResult)?.StatusCode, StatusCodes.Status500InternalServerError);
    }
}