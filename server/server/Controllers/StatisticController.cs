using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StadisticController : ControllerBase
{
    private readonly UserService _userService;
    private readonly BattleService _battleService;

    public StadisticController(UserService userService, BattleService battleService)
    {
        _userService = userService;
        _battleService = battleService;
    }

    [HttpGet("ranking")]
    public async Task<IActionResult> GetRankingAsync()
    {

        var users = await _userService.GetRankingAsync();

        return Ok(users);
    }

    //[Authorize]
    [HttpGet("userBattles/{userId}")]
    public async Task<IActionResult> GetPositionDistribution(int userId)
    {
        var battles = await _battleService.GetEndedBattlesByUserAsync(userId);
        return Ok(battles);
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
