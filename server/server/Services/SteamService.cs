using server.Models.DTOs;
using server.Models.Entities;
using server.Repositories;
using System.Text.Json;

namespace server.Services;

public class SteamService
{
    private readonly string _apiKey = Environment.GetEnvironmentVariable("STEAM_KEY");

    private readonly UnitOfWork _unitOfWork;

    public SteamService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task AssociateSteamAccountAsync(int userId, string verificationCode, string steamId)
    {
        User user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if(user == null || !user.VerificationCode.Equals(verificationCode))
        {
            return;
        }

        user.SteamId = steamId;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveAsync();

        Console.WriteLine("Asociado correctamente :D");
    }

    public async Task RemoveAccountFromMainAsync(User user)
    {
        user.SteamId = null;
        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveAsync();
    }

    public async Task<SteamProfile> GetPlayerSummaryAsync(string steamId)
    {
        using HttpClient httpClient = new HttpClient();

        var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId}";
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        var player = doc.RootElement
            .GetProperty("response")
            .GetProperty("players")[0];

        return new SteamProfile
        {
            SteamId = player.GetProperty("steamid").GetString(),
            PersonaName = player.GetProperty("personaname").GetString(),
            Avatar = player.GetProperty("avatar").GetString(),           // 32px
            AvatarFull = player.GetProperty("avatarfull").GetString()    // 184px
        };
    }
}