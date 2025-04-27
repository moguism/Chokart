using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BattleController : ControllerBase
{
    private readonly UserService _userService;
    private readonly BattleService _battleService;

    public BattleController(UserService userService, BattleService battleService)
    {
        _userService = userService;
        _battleService = battleService;
    }

    [Authorize]
    [HttpPost]
    public async Task CreateBattle([FromBody] BattlePetition battlePetition)
    {
        User user = await GetAuthorizedUser();

        if(user == null)
        {
            return;
        }

        await _battleService.CreateBattleAsync(battlePetition);
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
