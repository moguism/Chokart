using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.Entities;
using server.Services;
using server.Sockets;
using System.Net.WebSockets;

namespace server.Controllers;

[Route("socket")]
[ApiController]
public class WebSocketController : ControllerBase
{
    private readonly WebSocketHandler _webSocketHandler;
    private readonly UserService _userService;

    public WebSocketController(WebSocketHandler webSocketHandler, UserService userService)
    {
        _webSocketHandler = webSocketHandler;
        _userService = userService;
    }

    // TODO: Cambiar el nickname por el JWT, poner el Authorize, y cambiar todo lo relativo a username por User, como en Mixdrop

    [Authorize]
    [HttpGet]
    public async Task ConnectAsync()
    {
        Console.WriteLine("entro en el controlador");

        // Si la petición es de tipo websocket la aceptamos
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            Console.WriteLine("es peticion de websocket");

            // Aceptamos la solicitud
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            Console.WriteLine("acepto peticion");


            User user = await GetAuthorizedUser();

            if (user == null)
            {
                return;
            }

            Console.WriteLine("usuario no nulo " + user.Nickname);


            string ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            Console.WriteLine("IP: " + ip);

            // Manejamos la solicitud.
            await _webSocketHandler.HandleWebsocketAsync(webSocket, user, ip);
        }
        // En caso contrario la rechazamos
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }

        // Cuando este método finalice, se cerrará automáticamente la conexión con el websocket
    }

    private async Task<User> GetAuthorizedUser()
    {
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string firstClaim = currentUser.Claims.First().ToString();
        string idString = firstClaim.Substring(firstClaim.IndexOf("nameidentifier:") + "nameIdentifier".Length + 2);

        // Pilla el usuario de la base de datos
        User user = await _userService.GetBasicUserByIdAsync(int.Parse(idString));

        if (user == null || user.Banned)
        {
            Console.WriteLine("Usuario baneado");
            return null;
        }

        return user;
    }
}
