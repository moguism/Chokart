using server.Models.Entities;

namespace server.Sockets.Game;

public class GameHandler
{
    public readonly List<UserBattle> participants = new List<UserBattle>();
    public bool Started { get; set; } = false;

    public GameHandler(UserBattle host)
    {
        participants.Add(host);
    }
}
