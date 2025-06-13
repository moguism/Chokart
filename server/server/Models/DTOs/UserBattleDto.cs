using server.Models.Entities;

namespace server.Models.DTOs;

public class UserBattleDto
{
    public int CharacterId { get; set; }
    public int Punctuation { get; set; }
    public int BattleResultId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string UserImage { get; set; }
    public int  Position { get; set; }

    public long TimePlayed { get; set; } = 0;

    public int TotalKills { get; set; } = 0;
}
