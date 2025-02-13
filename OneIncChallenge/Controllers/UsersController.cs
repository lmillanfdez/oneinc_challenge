using Microsoft.AspNetCore.Mvc;
using OneIncChallenge.Exceptions;
using OneIncChallenge.Models.User;
using OneIncChallenge.Services.Contracts;

namespace OneIncChallenge.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUsersService _usersService;
    private readonly Serilog.ILogger _logger;

    public UsersController(IUsersService usersService)
    {
        _usersService = usersService;
        _logger = Serilog.Log.Logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var results = await _usersService.GetDetailedUsers();

            return Ok(results);
        }
        catch(Exception exc)
        {
            _logger.Error(exc, "Getting users");

            return StatusCode(StatusCodes.Status500InternalServerError, exc);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        if(!string.IsNullOrEmpty(id))
        {
            Guid userGuid;

            if(!Guid.TryParse(id, out userGuid))
            {
                _logger.Error($"Getting user with invalid id: {id}");
                return BadRequest(new { ErrorMessage = "Invalid user's id" });
            }

            try
            {
                var result = await _usersService.GetDetailedUser(userGuid);

                return Ok(result);
            }
            catch(Exception exc)
            {
                _logger.Error(exc, $"Getting user with id: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        _logger.Error("Getting user with null or empty id");
        return BadRequest(new { ErrorMessage = "User's id cannot be null or empty." });
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _usersService.AddUser(user);

            return Created($"/api/users/{user.Id}", user);
        }
        catch(InvalidUserException iuException)
        {
            _logger.Error(iuException, "Adding user");
            return BadRequest(new { ErrorMessage = iuException.Message });
        }
        catch(Exception exc)
        {
            _logger.Error(exc, "Adding user");
            return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserOrCreateIfNotExist(string id, [FromBody] UserPutRequestDTO user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(!string.IsNullOrEmpty(id))
        {
            Guid userGuid;

            if(!Guid.TryParse(id, out userGuid))
            {
                _logger.Error($"Updating user with invalid id: {id}");
                return BadRequest(new { ErrorMessage = "Invalid user's id" });
            }

            try
            {
                var result = await _usersService.UpdateUserOrCreateIfNotExist(userGuid, user);

                return result ? Created($"/api/users/{id}", user) : Ok();
            }
            catch(InvalidUserException iuException)
            {
                _logger.Error(iuException, $"Updating user with invalid id: {id}");
                return BadRequest(new { ErrorMessage = iuException.Message });
            }
            catch(Exception exc)
            {
                _logger.Error(exc, $"Updating user with invalid id: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        _logger.Error("Updating user with null or empty id");
        return BadRequest(new { ErrorMessage = "User's id cannot be null or empty." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if(!string.IsNullOrEmpty(id))
        {
            Guid userGuid;

            if(!Guid.TryParse(id, out userGuid))
            {
                _logger.Error($"Deleting user with invalid id: {id}");
                return BadRequest(new { ErrorMessage = "Invalid user's id" });
            }

            try
            {
                if(await _usersService.DeleteUser(userGuid))
                {
                    return Ok();
                }

                return NotFound();
            }
            catch(Exception exc)
            {
                _logger.Error(exc, $"Deleting user with invalid id: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, exc.Message);
            }
        }

        _logger.Error("Deleting user with null or empty id");
        return BadRequest(new { ErrorMessage = "User's id cannot be null or empty." });
    }
}