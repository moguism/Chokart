namespace server.Models.Entities;

public class Battle
{
    public int Id { get; set; }
    public bool IsAgainstBot { get; set; } = false;
    //public BattleState BattleState { get; set; }
    public int BattleStateId { get; set; } = 1;
    public ICollection<UserBattle> BattleUsers { get; set; } = new List<UserBattle>();
    public Track Track { get; set; } = null;
    public int TrackId { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime FinishedAt { get; set; } = DateTime.UtcNow.AddMonths(1); // Valor por defecto
}
