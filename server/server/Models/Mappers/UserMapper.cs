using server.Models.DTOs;
using server.Models.Entities;

namespace server.Models.Mappers;

public class UserMapper
{
    //Pasar de usuario a dto
    public UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Nickname = user.Nickname,
            Email = user.Email,
            Role = user.Role,
            AvatarPath = user.AvatarPath,
            //IsInQueue = user.IsInQueue,
            Banned = user.Banned,
            StateId = user.StateId,
            Friendships = user.Friendships,
            TotalPoints = user.TotalPoints,
            VerificationCode = user.VerificationCode,
            //BattleUsers = user.BattleUsers
            SteamId = user.SteamId
        };
    }

    public UserDtoRanking ToDtoRanking(User user)
    {
        return new UserDtoRanking
        {
            Id = user.Id,
            Nickname = user.Nickname,
            AvatarPath = user.AvatarPath,          
            TotalPoints = user.TotalPoints,
            SteamId = user.SteamId
        };
    }

    //Pasar la lista de usuarios a dtos
    public IEnumerable<UserDto> ToDto(IEnumerable<User> users)
    {
        return users.Select(ToDto);
    }

    //Para el ranking
    public IEnumerable<UserDtoRanking> ToDtoRanking(IEnumerable<User> users)
    {
        return users.Select(ToDtoRanking);
    }


}
