using System.Net.WebSockets;

namespace server.Sockets;

public class WebSocketHandler
{
    private static IServiceProvider _serviceProvider;
    public static readonly List<UserSocket> USER_SOCKETS = new List<UserSocket>();

    // Semáforo para controlar el acceso a la lista de WebSocketHandler
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public WebSocketHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task HandleWebsocketAsync(WebSocket webSocket, string user)
    {
        // Creamos un nuevo WebSocketHandler a partir del WebSocket recibido y lo añadimos a la lista
        UserSocket handler = await AddWebsocketAsync(webSocket, user);
        await handler.ProcessWebSocket();
    }

    private async Task<UserSocket> AddWebsocketAsync(WebSocket webSocket, string user)
    {
        // Esperamos a que haya un hueco disponible
        await _semaphore.WaitAsync();

        // Sección crítica

        UserSocket existingSocket = USER_SOCKETS.FirstOrDefault(u => u.User.Equals(user));
        if (existingSocket != null)
        {
            USER_SOCKETS.Remove(existingSocket);
        }

        UserSocket handler = new UserSocket(_serviceProvider, webSocket, user);
        handler.Disconnected += OnDisconnectedAsync;
        USER_SOCKETS.Add(handler);

        // Liberamos el semáforo
        _semaphore.Release();

        return handler;
    }

    private async Task OnDisconnectedAsync(UserSocket disconnectedHandler)
    {
        // Esperamos a que haya un hueco disponible
        await _semaphore.WaitAsync();

        // Sección crítica
        // Nos desuscribimos de los eventos y eliminamos el WebSocketHandler de la lista
        disconnectedHandler.Disconnected -= OnDisconnectedAsync;
        USER_SOCKETS.Remove(disconnectedHandler);

        // Liberamos el semáforo
        _semaphore.Release();
    }

    public static async Task NotifyOneUser(string jsonToSend, string nickname)
    {
        var userSocket = USER_SOCKETS.FirstOrDefault(userSocket => userSocket.User == nickname);
        if (userSocket != null && userSocket.Socket.State == WebSocketState.Open)
        {
            await userSocket.SendAsync(jsonToSend);
        }
    }

    public static async Task NotifyUsers(string jsonToSend)
    {
        foreach (var userSocket in USER_SOCKETS)
        {
            if (userSocket.Socket.State == WebSocketState.Open) await userSocket.SendAsync(jsonToSend);
        }
    }
}
