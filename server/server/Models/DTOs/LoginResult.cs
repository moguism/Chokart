namespace server.Models.DTOs;

// se envia al iniciar sesion
public class LoginResult
{
    public int Id { get; set; }

    public string AccessToken { get; set; }

    public UserDto User { get; set; }   // Para que el front reciba datos del usuario que ha iniciado sesión
}
