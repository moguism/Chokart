﻿using server.Models.Entities;
using server.Sockets.Game;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace server.Sockets;

public class UserSocket
{
    private static IServiceProvider _serviceProvider;

    public WebSocket Socket;
    public User User { get; set; }

    public event Func<UserSocket, Task> Disconnected;
    public string IpAddress { get; set; }

    public UserSocket(IServiceProvider serviceProvider, WebSocket socket, User user, string ipAddress)
    {
        _serviceProvider = serviceProvider;
        Socket = socket;
        User = user;
        IpAddress = ipAddress;
    }

    public async Task ProcessWebSocket()
    {
        // Mientras que el websocket del cliente esté conectado
        while (Socket.State == WebSocketState.Open)
        {
            try
            {
                string message = await ReadAsync();
                Dictionary<object, object> dictInput = GetActionMessage(message);


                // Si ha recibido algo
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (dictInput == null)
                    {
                        Console.WriteLine("El diccionario no es válido");
                        continue;
                    }

                    // AQUÍ TRASLADO EL MENSAJE AL ENUM Y HAGO SWITCH (POR AHORA) (ahora el messageType está en el catch de GetActionMessage()
                    JsonSerializerOptions options = new JsonSerializerOptions();
                    options.ReferenceHandler = ReferenceHandler.IgnoreCycles;

                    using IServiceScope scope = _serviceProvider.CreateScope();
                    dictInput.TryGetValue("messageType", out object messageTypeRaw);

                    MessageType messageType = (MessageType)messageTypeRaw;

                    Dictionary<object, object> dict = new Dictionary<object, object>
                    {
                        { "messageType", messageType },
                        { "joined", false }
                    };

                    bool send = true;

                    // En función del switch, obtengo unos datos u otros, y los envío en JSON
                    switch (messageType)
                    {
                        case MessageType.HostGame:
                            await GameNetwork.HostLobby(User);
                            dict.Add("participants", User.Nickname);
                            break;
                        case MessageType.JoinGame:
                            dict["joined"] = await GameNetwork.JoinLobby(User, dictInput["host"].ToString());
                            break;
                        case MessageType.StartGame:
                            dict["joined"] = GameNetwork.StartGame(User.Nickname);
                            break;
                        case MessageType.GameStarted:
                            send = false;
                            await GameNetwork.StartGameForClients(User.Nickname, IpAddress);
                            break;
                    }

                    if (send)
                    {
                        string outMessage = System.Text.Json.JsonSerializer.Serialize(dict, options);
                        // Procesamos el mensaje
                        //string outMessage = $"[{string.Join(", ", message as IEnumerable<char>)}]";

                        // Enviamos respuesta al cliente
                        await SendAsync(outMessage);
                    }

                    scope.Dispose();
                }
            }
            catch (Exception e)
            {
                if (e is System.FormatException)
                {
                    continue;
                }
                Console.WriteLine($"Error: {e}");
            }
        }

        // Si hay suscriptores al evento Disconnected, gestionamos el evento
        if (Disconnected != null)
        {
            await Disconnected.Invoke(this);
        }
    }

    public Dictionary<object, object> GetActionMessage(string message)
    {
        Dictionary<object, object> dict = new Dictionary<object, object>
        {
            { "messageType", -1 }
        };

        try
        {
            JsonDocument dxoc = JsonDocument.Parse(message);
            JsonElement elem = dxoc.RootElement;

            MessageType messageType = (MessageType)elem.GetProperty("messageType").GetInt32();
            dict["messageType"] = messageType;

            string host = elem.GetProperty("host").ToString();
            dict.Add("host", host);
        }
        catch {}

        return dict;
    }

    // TANTO READ COMO SEND SON COMUNES, Y SIEMPRE ENVÍAN Y RECIBEN STRINGS EN FORMATO JSON
    // READ RECIBIRÍA EL TIPO DEL MENSAJE (por ejemplo, que quiero info de las partidas), Y HABRÍA UNA O VARIAS CLASES QUE OBTENGA LOS DATOS QUE QUIERE Y ENVÍE LO NECESARIO
    private async Task<string> ReadAsync(CancellationToken cancellation = default)
    {
        // Creo un buffer para almacenar temporalmente los bytes del contenido del mensaje
        byte[] buffer = new byte[4096];
        // Creo un StringBuilder para poder ir creando poco a poco el mensaje en formato texto
        StringBuilder stringBuilder = new StringBuilder();
        // Creo un booleano para saber cuándo termino de leer el mensaje
        bool endOfMessage = false;

        do
        {
            // Recibo el mensaje pasándole el buffer como parámetro
            WebSocketReceiveResult result = await Socket.ReceiveAsync(buffer, cancellation);

            // Si el resultado que se ha recibido es de tipo texto lo decodifico y lo meto en el StringBuilder
            if (result.MessageType == WebSocketMessageType.Text)
            {
                // Decodifico el contenido recibido
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                // Lo añado al StringBuilder
                stringBuilder.Append(message);
            }
            // Si el resultado que se ha recibido entonces cerramos la conexión
            else if (result.CloseStatus.HasValue)
            {
                // Cerramos la conexión
                await Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, cancellation);
            }

            // Guardamos en nuestro booleano si hemos recibido el final del mensaje
            endOfMessage = result.EndOfMessage;
        }
        // Repetiremos iteración si el socket permanece abierto y no se ha recibido todavía el final del mensaje
        while (Socket.State == WebSocketState.Open && !endOfMessage);

        return stringBuilder.ToString();

        // Finalmente devolvemos el contenido del StringBuilder
        //return stringBuilder.ToString();
    }

    public Task SendAsync(string message, CancellationToken cancellation = default)
    {
        // Codificamos a bytes el contenido del mensaje
        byte[] bytes = Encoding.UTF8.GetBytes(message);

        // Enviamos los bytes al cliente marcando que el mensaje es un texto
        return Socket.SendAsync(bytes, WebSocketMessageType.Text, true, cancellation);
    }
}
