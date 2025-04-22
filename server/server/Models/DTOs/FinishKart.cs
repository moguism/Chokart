namespace server.Models.DTOs;

public class FinishKart
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; }
    public int Position { get; set; }
    public int Kills { get; set; }
}