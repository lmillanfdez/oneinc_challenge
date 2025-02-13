using OneIncChallenge.Exceptions;
using OneIncChallenge.Helpers;
using OneIncChallenge.Models.User;
using OneIncChallenge.Repositories.Contracts;
using OneIncChallenge.Services.Contracts;


namespace OneIncChallenge.Services.Implementations; 

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<IEnumerable<UserDetailsDTO>> GetDetailedUsers()
    {
        var results = await _usersRepository.GetDetailedUsers();

        return results;
    }

    public async Task<UserDetailsDTO?> GetDetailedUser(Guid id)
    {
        var result = await _usersRepository.GetDetailedUser(id);

        return result;
    }

    public async Task<User> AddUser(User user)
    {
        string errorMessage;

        if(!UsersHelper.IsValidUser(user, out errorMessage))
        {
            throw new InvalidUserException(errorMessage);
        }

        var result = await _usersRepository.AddUser(user);

        return result;
    }

    public async Task<bool> UpdateUserOrCreateIfNotExist(Guid userGuid, UserPutRequestDTO user)
    {
        string errorMessage;

        if(!UsersHelper.IsValidUser(user, out errorMessage))
        {
            throw new InvalidUserException(errorMessage);
        }

        return await _usersRepository.UpdateUserOrCreateIfNotExist(userGuid, user);
    }

    public async Task<bool> DeleteUser(Guid userGuid)
    {
        return await _usersRepository.DeleteUser(userGuid);
    }
}