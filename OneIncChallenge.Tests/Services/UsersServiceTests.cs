using Moq;
using OneIncChallenge.Exceptions;
using OneIncChallenge.Models.User;
using OneIncChallenge.Repositories.Contracts;
using OneIncChallenge.Services.Implementations;
using OneIncChallenge.Tests.Helpers;

namespace OneIncChallenge.Tests.Services;

public class UsersServiceTests
{
    public UsersServiceTests()
    {
    }

    [Fact]
    public async Task GetDetailedUsers_Returns_ExpectedTypeAndData()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var users = new List<UserDetailsDTO>
        {
            new ()
            {
                FirstName = "John",
                LastName = string.Empty,
                Email = "john@doe.com",
                DateOfBirth = DateTime.Now.AddYears(-20),
                PhoneNumber = 1234567890
            },
            new ()
            {
                FirstName = "Susan",
                LastName = string.Empty,
                Email = "susan@aaron.com",
                DateOfBirth = DateTime.Now.AddYears(-25),
                PhoneNumber = 9876543210
            }
        };

        usersRepositoryMock.Setup(ur => ur.GetDetailedUsers())
                            .Returns(Task.FromResult(users.AsEnumerable()));

        var usersService = new UsersService(usersRepositoryMock.Object);
    
        //act
        var validUsersAsString = SerializationHelper.SerializeResult(users);
        var results = await usersService.GetDetailedUsers();
    
        //assert
        Assert.NotNull(results);
        Assert.IsAssignableFrom<IEnumerable<UserDetailsDTO>>(results);
        Assert.Equal(validUsersAsString, SerializationHelper.SerializeResult(users));
    }

    [Fact]
    public async Task GetDetailedUser_Returns_ExpectedTypeAndData()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var validUser = new UserDetailsDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        usersRepositoryMock.SetupSequence(ur => ur.GetDetailedUser(It.IsAny<Guid>()))
                            .Returns(Task.FromResult<UserDetailsDTO?>(validUser))
                            .Returns(Task.FromResult<UserDetailsDTO?>(null));

        var usersService = new UsersService(usersRepositoryMock.Object);
        var userGuid = Guid.Parse("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");
    
        //act
        var validUserAsString = SerializationHelper.SerializeResult(validUser);
        var result = await usersService.GetDetailedUser(userGuid);
        var nullResult = await usersService.GetDetailedUser(userGuid);
    
        //assert
        Assert.Equal(validUserAsString, SerializationHelper.SerializeResult(result));
        Assert.Equal("null", SerializationHelper.SerializeResult(nullResult));
    }

    [Fact]
    public async Task AddUser_Returns_InvalidUserException_WithUnvalidUser()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var invalidUser = new User 
        {
            FirstName = null,
            LastName = "Doe",
            Email = "john@com",
            DateOfBirth = DateTime.Now.AddYears(-15),
            PhoneNumber = 0
        };

        usersRepositoryMock.Setup(ur => ur.AddUser(It.IsAny<User>()))
                            .Returns(Task.FromResult(invalidUser));

        var usersService = new UsersService(usersRepositoryMock.Object);
    
        //act
        var result = usersService.AddUser(invalidUser);
    
        //assert
        await Assert.ThrowsAsync<InvalidUserException>(() => result);
    }

    [Fact]
    public async Task AddUser_Returns_ExpectedTypeAndData()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var validUser = new User 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        usersRepositoryMock.Setup(ur => ur.AddUser(It.IsAny<User>()))
                            .Returns(Task.FromResult(validUser));

        var usersService = new UsersService(usersRepositoryMock.Object);
    
        //act
        var validUserAsString = SerializationHelper.SerializeResult(validUser);
        var result = await usersService.AddUser(validUser);
    
        //assert
        Assert.NotNull(result);
        Assert.Equal(validUserAsString, SerializationHelper.SerializeResult(result));
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_InvalidUserException_WithUnvalidUser()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var invalidUser = new UserPutRequestDTO 
        {
            FirstName = null,
            LastName = "Doe",
            Email = "john@com",
            DateOfBirth = DateTime.Now.AddYears(-15),
            PhoneNumber = 0
        };

        usersRepositoryMock.Setup(ur => ur.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                            .Returns(Task.FromResult(true));

        var usersService = new UsersService(usersRepositoryMock.Object);
        var userGuid = Guid.Parse("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");
    
        //act
        var result = usersService.UpdateUserOrCreateIfNotExist(userGuid, invalidUser);
    
        //assert
        await Assert.ThrowsAsync<InvalidUserException>(() => result);
    }

    [Fact]
    public async Task UpdateUserOrCreateIfNotExist_Returns_ExpectedTypeAndData()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();
        var validUser = new UserPutRequestDTO 
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@doe.com",
            DateOfBirth = DateTime.Now.AddYears(-20),
            PhoneNumber = 1234567890
        };

        usersRepositoryMock.SetupSequence(ur => ur.UpdateUserOrCreateIfNotExist(It.IsAny<Guid>(), It.IsAny<UserPutRequestDTO>()))
                            .Returns(Task.FromResult(true))
                            .Returns(Task.FromResult(false));

        var usersService = new UsersService(usersRepositoryMock.Object);
        var userGuid = Guid.Parse("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");
    
        //act
        var trueResult = await usersService.UpdateUserOrCreateIfNotExist(userGuid, validUser);
        var falseResult = await usersService.UpdateUserOrCreateIfNotExist(userGuid, validUser);
    
        //assert
        Assert.True(trueResult);
        Assert.False(falseResult);
    }

    [Fact]
    public async Task DeleteUser_Returns_ExpectedTypeAndData()
    {
        //arrange
        var usersRepositoryMock = new Mock<IUsersRepository>();

        usersRepositoryMock.SetupSequence(ur => ur.DeleteUser(It.IsAny<Guid>()))
                            .Returns(Task.FromResult(true))
                            .Returns(Task.FromResult(false));

        var usersService = new UsersService(usersRepositoryMock.Object);
        var userGuid = Guid.Parse("1214ECC2-8B8F-48BF-906E-2A7B2AC064B4");
    
        //act
        var trueResult = await usersService.DeleteUser(userGuid);
        var falseResult = await usersService.DeleteUser(userGuid);
    
        //assert
        Assert.True(trueResult);
        Assert.False(falseResult);
    }
}