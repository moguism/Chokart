using server.Models.Entities;

namespace server.Models.DTOs;

// sin contraseña
public class UserDtoRanking
{
    public int Id { get; set; }
    public string Nickname { get; set; }
    public string AvatarPath { get; set; }
    public long TotalPoints { get; set; } = 0;
    public string SteamId { get; set; }
}
