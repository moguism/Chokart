using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Models.DTOs;
using server.Models.Entities;
using server.Services;

namespace server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly FriendshipService _friendshipService;

    public UserController(UserService userService, FriendshipService friendshipService)
    {
        _userService = userService;
        _friendshipService = friendshipService;
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<UserDto> GetUserById(int id)
    {
        User user = await GetAuthorizedUser();

        if(user == null)
        {
            return null;
        }

        UserDto userResponse;

        var friendships = await _friendshipService.GetFriendList(id);

        if(user.Id != id)
        {
            User askedUser = await _userService.GetBasicUserByIdAsync(id);
            askedUser.Friendships = friendships;
            userResponse = _userService.ToDto(askedUser);
        }
        else
        {
            user.Friendships = friendships;
            userResponse = _userService.ToDto(user);
        }

        return userResponse;
    }

    [Authorize]
    [HttpGet("allUsers")]
    public async Task<IActionResult> GetAllUsersAsync()
    {

        User currentUser = await GetAuthorizedUser();

        if (currentUser == null)
        {
            return null;
        }

        var users = await _userService.GetAllUsersAsync(currentUser);

        return Ok(users);
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> SearchUser([FromQuery] string query)
    {

        User currentUser = await GetAuthorizedUser();

        if (currentUser == null)
        {
            return null;
        }

        if (query == null)
        {
            return BadRequest("Busqueda fallida.");
        }

        var result = await _userService.SearchUser(query);

        result.Remove(result.Find(user => user.Id == currentUser.Id));

        if (result.Count == 0)
        {
            return Ok(new { users = new List<UserDto>() });
        }


        return Ok(new { users = result });

    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<UserDto> UpdateUser([FromForm] RegisterDto user, int id)
    {
        try
        {
            user.Id = id;

            User currentUser = await GetAuthorizedUser();

            if (currentUser == null)
            {
                return null;
            }

            // Si no es admin y está intentando modificar a otro usuario
            if (!currentUser.Role.Equals("Admin") && user.Id != currentUser.Id)
            {
                return null;
            }

            string role = "User";
            if (currentUser.Role.Equals("Admin"))
            {
                role = user.Role;
            }

            if (currentUser.Id == user.Id)
            {
                return await _userService.UpdateUser(user, currentUser, role);
            }
            else
            {
                // Para los admin
                User oldUser = await _userService.GetBasicUserByIdAsync(user.Id);
                return await _userService.UpdateUser(user, oldUser, role);
            }
        }
        catch
        {
            return null;
        }
    }

    // Solo pueden usar este método los usuarios cuyo rol sea admin
    [Authorize(Roles = "Admin")]
    [HttpPut("modifyUserRole")]
    public async Task<IActionResult> ModifyUserRole(ModifyRoleRequest request)
    {
        try
        {
            if (request.NewRole == "User" || request.NewRole == "Admin")
            {
                await _userService.ModifyUserRoleAsync(request.UserId, request.NewRole);
                return Ok("Rol de usuario actualizado correctamente.");
            }
            else
            {
                return BadRequest("El nuevo rol debe ser User o Admin");
            }

        }
        catch (InvalidOperationException)
        {
            return BadRequest("No pudo modificarse el rol del usuario.");
        }
    }

    // Elimina un usuario
    [Authorize(Roles = "Admin")]
    [HttpPut("banUser/{userId}")]
    public async Task<IActionResult> BanUser(int userId)
    {
        try
        {
            await _userService.BanUserAsync(userId);

            return Ok("Usuario baneado correctamente.");
        }
        catch (InvalidOperationException)
        {
            return BadRequest("No se pudo banear al usuario");
        }
    }

    private async Task<User> GetAuthorizedUser()
    {
        System.Security.Claims.ClaimsPrincipal currentUser = this.User;
        string firstClaim = currentUser.Claims.First().ToString();
        string idString = firstClaim.Substring(firstClaim.IndexOf("nameidentifier:") + "nameIdentifier".Length + 2);

        // Pilla el usuario de la base de datos
        User user = await _userService.GetBasicUserByIdAsync(int.Parse(idString));

        if (user == null || user.Banned || !user.Verified)
        {
            Console.WriteLine("Usuario baneado");
            return null;
        }

        return user;
    }
}
