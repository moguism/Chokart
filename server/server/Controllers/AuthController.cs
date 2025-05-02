using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using server.Models.DTOs;
using server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly TokenValidationParameters _tokenParameters;

        private readonly UserService _userService;
        private readonly FriendshipService _friendshipService;

        private readonly EmailService _emailService = new EmailService();

        public AuthController(UserService userService, IOptionsMonitor<JwtBearerOptions> jwtOptions, FriendshipService friendshipService)
        {
            _userService = userService;
            _tokenParameters = jwtOptions.Get(JwtBearerDefaults.AuthenticationScheme).TokenValidationParameters;
            _friendshipService = friendshipService;
        }

        // CREAR NUEVO USUARIO
        [HttpPost("register")]
        public async Task<ActionResult<RegisterDto>> SignUp([FromBody] RegisterDto model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica si ya existe un usuario con el mismo correo
            var existingEmail = await _userService.GetUserByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return Conflict("El correo electrónico ya está en uso.");
            }

            // Verifica si ya existe un usuario con el apodo
            var existingNickname = await _userService.GetUserByNicknameAsync(model.Nickname);
            if (existingNickname != null)
            {
                return Conflict("El nickname ya está en uso.");
            }

            var newUser = await _userService.RegisterAsync(model);

            var userDto = _userService.ToDto(newUser);

            // Mando un correo de verifición
            await _emailService.CreateEmailUser(newUser.Email, newUser.Id, newUser.VerificationCode);

            return CreatedAtAction(nameof(Login), new { email = userDto.Email }, userDto);
        }

        // LOGIN 
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest model)
        {
            try
            {
                // Se usa el método LoginAsync para verificar el usuario y la contraseña
                var user = await _userService.LoginAsync(model);

                // Si el usuario es null, se devuelve Unauthorized
                if (user == null || user.Banned || !user.Verified)
                {
                    return Unauthorized("Datos de inicio de sesión incorrectos.");
                }

                var friendships = await _friendshipService.GetFriendList(user.Id);
                user.Friendships = friendships;

                // Se crea el token JWT
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    // Se añaden los datos que autorizan al usuario
                    Claims = new Dictionary<string, object>
                {
                    { ClaimTypes.NameIdentifier, user.Id },      // ID del usuario
                    { ClaimTypes.Name, user.Nickname},           // Apodo
                    { ClaimTypes.Role, user.Role }               // Rol del usuario
                },
                    // Expiración del token en 5 años
                    Expires = DateTime.UtcNow.AddYears(5),

                    // Clave y algoritmo de firmado
                    SigningCredentials = new SigningCredentials(
                        _tokenParameters.IssuerSigningKey,
                        SecurityAlgorithms.HmacSha256Signature)
                };

                // Creación del token
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string stringToken = tokenHandler.WriteToken(token);

                // Se devuelve el resultado de inicio de sesión con el token y los datos del usuario
                var loginResult = new LoginResult
                {
                    Id = user.Id,
                    AccessToken = stringToken,
                    User = _userService.ToDto(user)
                };

                return Ok(loginResult);
            }

            catch (InvalidOperationException)
            {
                // Si hay algún error, se devuelve Unauthorized
                return Unauthorized("Datos de inicio de sesión incorrectos.");
            }
        }

        [HttpGet("verify/{id}/{code}")]
        public async Task<bool> VerifyUser(int id, string code)
        {
            bool couldUpdate = await _userService.VerifyUserAsync(id, code);
            return couldUpdate;
        }
    }
}
