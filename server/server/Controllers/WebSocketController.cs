using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Sockets;
using System.Net.WebSockets;

namespace server.Controllers;

[Route("socket")]
[ApiController]
public class WebSocketController : ControllerBase
{
    private readonly WebSocketHandler _webSocketHandler;

    public WebSocketController(WebSocketHandler webSocketHandler)
    { 
        _webSocketHandler = webSocketHandler;
    }

    // TODO: Cambiar el nickname por el JWT, poner el Authorize, y cambiar todo lo relativo a username por User, como en Mixdrop

    [Route("{nickname}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task ConnectAsync(string nickname)
    {
        // Si la petición es de tipo websocket la aceptamos
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            // Aceptamos la solicitud
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // Manejamos la solicitud.
            await _webSocketHandler.HandleWebsocketAsync(webSocket, nickname);
        }
        // En caso contrario la rechazamos
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        // Cuando este método finalice, se cerrará automáticamente la conexión con el websocket
    }
}
