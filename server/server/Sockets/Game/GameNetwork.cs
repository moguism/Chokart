using System.Text.Json.Serialization;
using System.Text.Json;
using server.Models.Entities;

namespace server.Sockets.Game;

public class GameNetwork
{
    public static readonly List<GameHandler> handlers = new List<GameHandler>();
    public static readonly List<User> usersWaiting = new List<User>();
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public static async Task HostLobby(User user)
    {
        await _semaphore.WaitAsync();

        UserBattle userBattle = CreateUserBattle(user, true);
        GameHandler handler = new GameHandler(userBattle);

        User[] waitingCopy = usersWaiting.ToArray();

        // En cuanto alguien hostea una partida, mete a la gente que estaba esperando
        foreach (User userWaiting in waitingCopy)
        {
            if(handler.participants.Count == GameHandler.MaxPlayers)
            {
                break;
            }

            UserBattle newParticipant = CreateUserBattle(userWaiting, false);
            handler.participants.Add(newParticipant);
            usersWaiting.Remove(userWaiting);
        }

        // Por si el host estaba esperando a encontrar partida
        usersWaiting.Remove(user);

        handlers.Add(handler);

        Dictionary<object, object> dict = new Dictionary<object, object>()
        {
            { "messageType", MessageType.PlayerJoined },
            { "participants", handler.participants.Select(p => p.User.Nickname) }
        };

        await SendParticipants(handler.participants, dict);

        _semaphore.Release();
    }

    public static async Task<bool> JoinLobby(User user, string hostName)
    {
        await _semaphore.WaitAsync();

        GameHandler handler = handlers.FirstOrDefault(h => h.participants.Any(p => p.UserId == user.Id));

        // Si se intenta meter en dos batallas a la vez
        if (handler != null)
        {
            _semaphore.Release();
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
            // Si no hay ninguna sala disponible lo pongo en espera
            usersWaiting.Add(user);
            _semaphore.Release();
            return false;
        }

        UserBattle userBattle = CreateUserBattle(user, false);
        handler.participants.Add(userBattle);

        Dictionary<object, object> dict = new Dictionary<object, object>()
        {
            { "messageType", MessageType.PlayerJoined },
            { "participants", handler.participants.Select(p => p.User.Nickname) }
        };

        await SendParticipants(handler.participants, dict);

        _semaphore.Release();

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

    public static async Task StartGameForClients(string hostName, string ip)
    {
        GameHandler handler = handlers.FirstOrDefault(h => h.Started == true && h.participants.Any(p => p.User.Nickname.Equals(hostName)));
        if (handler == null)
        {
            return;
        }

        Dictionary<object, object> dict = new Dictionary<object, object>()
        {
            { "messageType", MessageType.GameStarted },
            { "ip", ip }
        };

        // Notifico a todos menos al host porque ya está conectado
        await SendParticipants(handler.participants.Where(p => p.User.Nickname != hostName).ToList(), dict);
    }

    private static UserBattle CreateUserBattle(User user, bool isHost)
    {
        return new UserBattle()
        {
            User = user,
            UserId = user.Id,
            IsHost = isHost
        };
    }

    private static async Task SendParticipants(List<UserBattle> participants, Dictionary<object, object> dict)
    {
        JsonSerializerOptions options = new JsonSerializerOptions();
        options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        foreach (UserBattle participant in participants)
        {
            await WebSocketHandler.NotifyOneUser(JsonSerializer.Serialize(dict, options), participant.UserId);
        }
    }
}
