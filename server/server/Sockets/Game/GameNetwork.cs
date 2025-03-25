using System.Text.Json.Serialization;
using System.Text.Json;
using server.Models.Entities;

namespace server.Sockets.Game;

public class GameNetwork
{
    public static readonly List<GameHandler> handlers = new List<GameHandler>();

    public static void HostLobby(User user)
    {
        UserBattle userBattle = new UserBattle
        {
            User = user,
            UserId = user.Id,
            IsHost = true
        };

        handlers.Add(new GameHandler(userBattle));
    }

    public static async Task<bool> JoinLobby(User user, string hostName)
    {
        GameHandler handler = handlers.FirstOrDefault(h => h.participants.Any(p => p.UserId == user.Id));

        // Si se intenta meter en dos batallas a la vez
        if (handler != null)
        {
            return false;
        }

        if(hostName == "")
        {
            handler = handlers.FirstOrDefault(h => h.Started == false);
        }
        else
        {
            // Si que quiere unir a una lobby en concreto, especifica el nombre del host (que es quien le ha invitado)
            handler = handlers.FirstOrDefault(h => h.Started == false && h.participants.Any(p => p.User.Nickname.Equals(hostName)));
        }

        if(handler == null)
        {
            return false;
        }

        UserBattle userBattle = new UserBattle
        {
            User = user,
            UserId = user.Id,
            IsHost = false
        };

        handler.participants.Add(userBattle);

        JsonSerializerOptions options = new JsonSerializerOptions();
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        // TODO: CONVERTIR EN DTO
        Dictionary<object, object> dict = new Dictionary<object, object>()
        {
            { "messageType", MessageType.PlayerJoined },
            { "participants", handler.participants.Select(p => p.User.Nickname) }
        };

        foreach (UserBattle participant in handler.participants)
        {
            await WebSocketHandler.NotifyOneUser(JsonSerializer.Serialize(dict, options), participant.UserId);
        }

        return true;
    }

    // Esto le permite al host empezar la partida
    public static bool StartGame(string hostName)
    {
        GameHandler handler = handlers.FirstOrDefault(h => h.Started == false && h.participants.Any(p => p.User.Nickname.Equals(hostName)));
        if (handler == null)
        {
            return false;
        }
        handler.Started = true;
        return true;
    }
}
