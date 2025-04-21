namespace server.Models.Entities;

public class UserBattle
{
    public int Id { get; set; }
    public int Punctuation { get; set; } = 0;
    public long TimePlayed { get; set; } = 0;
    public int UserId { get; set; }
    public int BattleResultId { get; set; } = 1;
    public bool Receiver { get; set; } = false;
    public User User { get; set; } = null;
    public BattleResult BattleResult { get; set; } = null;
    public bool IsHost { get; set; } = false;
    public Character Character { get; set; } = null;
    public int CharacterId { get; set; } = 1;

    public int Position { get; set; } = 0;
    public int TotalKills { get; set; } = 0;
}
