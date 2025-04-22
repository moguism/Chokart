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

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<UserResponse> GetUserNicknameById(int id)
    {
        User user = await GetAuthorizedUser();

        if(user == null)
        {
            return null;
        }

        UserResponse userResponse;

        if(user.Id != id)
        {
            User askedUser = await _userService.GetBasicUserByIdAsync(id);
            userResponse = _userService.ToUserResponse(askedUser);
        }
        else
        {
            userResponse = _userService.ToUserResponse(user);
        }

        return userResponse;
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
