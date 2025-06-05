using Microsoft.EntityFrameworkCore;
using server.Models.DTOs;
using server.Models.Entities;
using server.Models.Helper;
using server.Models.Mappers;
using server.Repositories;
using server.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace server.Services;

public class UserService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly UserMapper _userMapper;
    private readonly EmailService _emailService = new EmailService();

    Regex emailRegex = new(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    public UserService(UnitOfWork unitOfWork, UserMapper userMapper)
    {
        _unitOfWork = unitOfWork;
        _userMapper = userMapper;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(User currentUser)
    {
        var users = await _unitOfWork.UserRepository.GetAllUsersAsync(currentUser.Id);
        return _userMapper.ToDto(users).ToList();
    }

    public async Task<UserDto> GetUserByNicknameAsync(string nickname)
    {
        var user = await _unitOfWork.UserRepository.GetByNicknameAsync(nickname);
        if (user == null)
        {
            return null;
        }
        return _userMapper.ToDto(user);
    }

    public async Task<User> GetBasicUserByIdAsync(int id)
    {
        User user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        return user;
    }

    public async Task<User> GetFullUserByIdAsync(int userId)
    {
        return await _unitOfWork.UserRepository.GetUserById(userId);
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return null;
        }
        return _userMapper.ToDto(user);
    }

    // REGISTRO 
    public async Task<User> RegisterAsync(RegisterDto model)
    {
        var state = await _unitOfWork.StateRepositoty.GetByIdAsync(2);

        if (state == null)
        {
            throw new Exception("El estado con ID 2 no existe en la base de datos.");
        }

        model.Email = model.Email?.Trim().ToLower();
        Console.WriteLine("Email recibido: " + model.Email);

        // validacion email

        if (string.IsNullOrEmpty(model.Email) || !emailRegex.IsMatch(model.Email))
        {
            throw new Exception("Email no valido.");
        }

        if (model.Password == null || model.Password.Length < 6)
        {
            throw new Exception("Contraseña no válida");
        }

        try
        {

            // Verifica si el usuario ya existe
            var existingUser = await GetUserByEmailAsync(model.Email.ToLower());

            if (existingUser != null)
            {
                throw new Exception("El usuario ya existe.");
            }

            //ImageService imageService = new ImageService();

            Guid uuid = Guid.NewGuid();
            string uuidString = uuid.ToString();

            var newUser = new User
            {
                Email = model.Email.ToLower(),
                Nickname = model.Nickname.ToLower(),
                //AvatarPath = "",
                Role = "User", // Rol por defecto
                Password = PasswordHelper.Hash(model.Password),
                //IsInQueue = false,  // por defecto al crearse
                StateId = state.Id,
                State = state,
                VerificationCode = uuidString
            };

            //if (model.Image != null)
            //{
            //    newUser.AvatarPath = "/" + await imageService.InsertAsync(model.Image);
            //}
            //else
            //{
            //    newUser.AvatarPath = "/avatar/user.png";
            //}

            await _unitOfWork.UserRepository.InsertAsync(newUser);
            await _unitOfWork.SaveAsync();

            return newUser;

        }
        catch (DbUpdateException ex)
        {
            // Log más detallado del error
            Console.WriteLine($"Error al guardar el usuario: {ex.InnerException?.Message}");
            throw new Exception("Error al registrar el usuario. Verifica los datos ingresados.");
        }
    }

    // INICIO DE SESION
    public async Task<User> LoginAsync(LoginRequest loginRequest)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailOrNickname(loginRequest.EmailOrNickname.ToLower());

        if (user == null || user.Password != PasswordHelper.Hash(loginRequest.Password))
        {
            return null;
        }

        return user;
    }

    public async Task<bool> VerifyUserAsync(int userId, string code)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if(user == null || !user.VerificationCode.Equals(code))
        {
            return false;
        }

        user.Verified = true;

        _unitOfWork.UserRepository.Update(user);
        await _unitOfWork.SaveAsync();

        return true;
    }

    // Actualizar usuario
    public async Task<UserDto> UpdateUser(RegisterDto model, User existingUser, string role)
    {
        // validacion email

        if (!emailRegex.IsMatch(model.Email))
        {
            throw new Exception("Email no valido.");
        }

        if (model.Password != null && model.Password != "" && model.Password.Length < 6)
        {
            throw new Exception("La contraseña no es válida");
        }

        try
        {
            bool sendEmail = false;

            // Verifica si el usuario ya existe
            if (!model.Email.Equals(existingUser.Email))
            {
                var otherUser = await GetUserByEmailAsync(model.Email.ToLower());

                if (otherUser != null)
                {
                    throw new Exception("El usuario ya existe.");
                }

                sendEmail = true;
            }

            ImageService imageService = new ImageService();

            existingUser.Email = model.Email.ToLower();
            existingUser.Nickname = model.Nickname.ToLower();
            existingUser.Role = role;

            if (model.Password != null && model.Password != "")
            {
                existingUser.Password = PasswordHelper.Hash(model.Password);
            }

            if(sendEmail)
            {
                existingUser.Verified = false;
                await _emailService.CreateEmailUser(existingUser.Email, existingUser.Id, existingUser.VerificationCode);
            }

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveAsync();

            UserSocket socket = WebSocketHandler.USER_SOCKETS.FirstOrDefault(u => u.User.Id == existingUser.Id);
            if (socket != null)
            {
                socket.User = existingUser;
            }

            return _userMapper.ToDto(existingUser);

        }
        catch (DbUpdateException ex)
        {
            // Log más detallado del error
            Console.WriteLine($"Error al guardar el usuario: {ex.InnerException?.Message}");
            throw new Exception("Error al registrar el usuario. Verifica los datos ingresados.");
        }
    }

    // Modificar el rol del usuario
    public async Task ModifyUserRoleAsync(int userId, string newRole)
    {
        var existingUser = await _unitOfWork.UserRepository.GetUserById(userId);


        if (existingUser == null)
        {
            throw new InvalidOperationException("Usuario con ID:" + userId + "no encontrado.");
        }

        // Console.WriteLine("ID del usuario: " + existingUser.Id);

        if (!string.IsNullOrEmpty(newRole))
        {
            existingUser.Role = newRole;
        }
        else
        {
            return;
        }

        UserSocket socket = WebSocketHandler.USER_SOCKETS.FirstOrDefault(u => u.User.Id == userId);
        if (socket != null)
        {
            socket.User.Role = newRole;
        }

        _unitOfWork.UserRepository.Update(existingUser);
        await _unitOfWork.SaveAsync();
    }

    // Banear usuario
    public async Task BanUserAsync(int userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);

        if (user == null)
        {
            throw new InvalidOperationException("El usuario no existe.");
        }

        user.Banned = !user.Banned;
        Console.WriteLine("Estado de baneo: " + user.Banned);

        _unitOfWork.UserRepository.Update(user);

        await _unitOfWork.SaveAsync();
    }

    // Buscar usuario
    public async Task<List<UserDto>> SearchUser(string search)
    {

        string searchSinTildes = Regex.Replace(search.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");

        var users = await _unitOfWork.UserRepository.SearchUser(searchSinTildes.ToLower());

        return _userMapper.ToDto(users).ToList();
    }

    public async Task<List<UserDtoRanking>> GetRankingAsync()
    {
        var ranking = await _unitOfWork.UserRepository.GetRankingAsync();
        return _userMapper.ToDtoRanking(ranking).ToList(); ;
    }

    public UserDto ToDto(User user)
    {
        return _userMapper.ToDto(user);
    }
}
