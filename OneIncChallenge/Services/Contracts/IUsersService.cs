using OneIncChallenge.Models.User;

namespace OneIncChallenge.Services.Contracts;

public interface IUsersService
{
    Task<IEnumerable<UserDetailsDTO>> GetDetailedUsers();
    Task<UserDetailsDTO?> GetDetailedUser(Guid id);
    Task<User> AddUser(User user);
    Task<bool> UpdateUserOrCreateIfNotExist(Guid userGuid, UserPutRequestDTO user);
    Task<bool> DeleteUser(Guid userGuid);
}