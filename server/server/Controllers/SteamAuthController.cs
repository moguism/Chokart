using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;
using System.Security.Claims;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SteamAuthController : ControllerBase
{
    private readonly SteamService _steamService;
    private readonly UserService _userService;

    public SteamAuthController(SteamService steamService, UserService userService)
    {
        _steamService = steamService;
        _userService = userService;
    }

    [HttpGet("getId/{id}")]
    public async Task<SteamProfile> GetProfile(string id)
    {
        SteamProfile profile = await _steamService.GetPlayerSummaryAsync(id);

        Console.WriteLine($"Nombre: {profile.PersonaName}");
        Console.WriteLine($"Avatar: {profile.AvatarFull}");

        return profile;
    }

    [Authorize]
    [HttpGet("remove")]
    public async Task RemoveSteamAccountFromMain()
    {
        User user = await GetAuthorizedUser();
        if (user == null)
        {
            return;
        }

        await _steamService.RemoveAccountFromMainAsync(user);
    }

    [HttpGet("login/{id}/{verification}")]
    public IActionResult Login(int id, string verification)
    {
        return Challenge(new AuthenticationProperties { RedirectUri = $"/api/SteamAuth/steam-callback/{id}/{verification}" }, "Steam");
    }

    [HttpGet("steam-callback/{id}/{verification}")]
    public async Task<IActionResult> SteamCallback(int id, string verification)
    {
        var result = await HttpContext.AuthenticateAsync("SteamCookie");

        if (!result.Succeeded)
            return Unauthorized();

        // El SteamID viene en este claim:
        var steamIdClaim = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(steamIdClaim))
        {
            return Unauthorized();
        }

        // Steam da una URL del palo: "https://steamcommunity.com/openid/id/76561198000000000"
        var steamId = steamIdClaim.Split('/').Last();

        Console.WriteLine($"Usuario inició sesión con SteamID: {steamId}");

        await _steamService.AssociateSteamAccountAsync(id, verification, steamId);

#if DEBUG
        return Redirect($"http://localhost:4200/profile?steamId={steamId}");
#else
        return Redirect($"https://www.playchokart.com/profile?steamId={steamId}");
#endif
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