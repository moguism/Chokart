namespace server.Sockets.Game;

public class GameHandler
{
    public readonly List<string> participants = new List<string>();

    public GameHandler(string hostName)
    {
        participants.Add(hostName);
    }
}
