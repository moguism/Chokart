using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StadisticController : ControllerBase
{
    private readonly UserService _userService;

    public StadisticController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("ranking")]
    public async Task<IActionResult> GetRankingAsync()
    {

        var users = await _userService.GetRankingAsync();

        return Ok(users);
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
