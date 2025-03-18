using server.Models.DTOs;
using server.Models.Entities;

namespace server.Models.Mappers;

public class UserBattleMapper
{
    //Pasar de usuario a dto
    public UserBattleDto ToDto(UserBattle user)
    {
        return new UserBattleDto
        {
            Punctuation = user.Punctuation,
            BattleResultId = user.BattleResultId,
            UserId = user.UserId,
            UserName = user.User.Nickname,
            UserImage = user.User.AvatarPath,
            CharacterId = user.CharacterId
        };
    }

    public IEnumerable<UserBattleDto> ToDto(ICollection<UserBattle> userBattles)
    {
        return userBattles.Select(ToDto);
    }
}
