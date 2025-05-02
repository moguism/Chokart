using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly FriendshipService _friendshipService;

    public UserController(UserService userService, FriendshipService friendshipService)
    {
        _userService = userService;
        _friendshipService = friendshipService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<UserDto> GetUserById(int id)
    {
        User user = await GetAuthorizedUser();

        if(user == null)
        {
            return null;
        }

        UserDto userResponse;

        var friendships = await _friendshipService.GetFriendList(id);

        if(user.Id != id)
        {
            User askedUser = await _userService.GetBasicUserByIdAsync(id);
            askedUser.Friendships = friendships;
            userResponse = _userService.ToDto(askedUser);
        }
        else
        {
            user.Friendships = friendships;
            userResponse = _userService.ToDto(user);
        }

        return userResponse;
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string query)
    {

        User currentUser = await GetAuthorizedUser();

        if (currentUser == null)
        {
            return null;
        }

        if (query == null)
        {
            return BadRequest("Busqueda fallida.");
        }

        var result = await _userService.SearchUser(query);

        result.Remove(result.Find(user => user.Id == currentUser.Id));

        if (result.Count == 0)
        {
            return Ok(new { users = new List<UserDto>() });
        }


        return Ok(new { users = result });

    }
    private async Task<User> GetAuthorizedUser()
    {
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string firstClaim = currentUser.Claims.First().ToString();
        string idString = firstClaim.Substring(firstClaim.IndexOf("nameidentifier:") + "nameIdentifier".Length + 2);

        // Pilla el usuario de la base de datos
        User user = await _userService.GetBasicUserByIdAsync(int.Parse(idString));

        if (user == null || user.Banned || !user.Verified)
        {
            Console.WriteLine("Usuario baneado");
            return null;
        }

        return user;
    }
}
