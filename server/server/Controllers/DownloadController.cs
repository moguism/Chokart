using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DownloadController : ControllerBase
{
    private readonly TokenValidationParameters _tokenParameters;

    private readonly UserService _userService;
    private readonly FriendshipService _friendshipService;

    private readonly EmailService _emailService = new EmailService();

    public DownloadController()
    {
    }

    // [Authorize] // si queremos que este logueado
    [HttpGet("{platform}")]
    public IActionResult DownloadFile(string platform)
    {
        string folder = "GameDownloadFiles";
        string filePath = "";

        switch (platform.ToLower())
        {
                //android
            case "apk":
                filePath = Path.Combine(folder, "chokart.apk");
                break;
                //windows
            case "exe":
                filePath = Path.Combine(folder, "chokart.exe");
                break;
                //iphone
            case "ipa":
                filePath = Path.Combine(folder, "chokart.ipa");
                break;
                //mac
            case "app":
                filePath = Path.Combine(folder, "chokart.app");
                break;
            default:
                return NotFound("Plataforma no válida");
        }

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Archivo no encontrado");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        var fileName = Path.GetFileName(filePath);

        return File(fileBytes, "application/octet-stream", fileName);
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
