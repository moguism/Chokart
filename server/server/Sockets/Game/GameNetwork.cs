using System.Text.Json.Serialization;
using System.Text.Json;

namespace server.Sockets.Game;

public class GameNetwork
{
    public static readonly List<GameHandler> handlers = new List<GameHandler>();

    public static void HostLobby(string userName)
    {
        handlers.Add(new GameHandler(userName));
    }

    public static async Task JoinLobby(string userName)
    {
        GameHandler handler = handlers.FirstOrDefault();
        handler.participants.Add(userName);

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        Dictionary<object, object> dict = new Dictionary<object, object>()
        {
            { "messageType", MessageType.PlayerJoined },
            { "participants", handler.participants }
        };

        foreach (string participant in handler.participants)
        {
            await WebSocketHandler.NotifyOneUser(JsonSerializer.Serialize(dict, options), participant);
        }
    }
}
