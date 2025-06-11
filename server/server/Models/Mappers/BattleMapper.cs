using server.Models.DTOs;
using server.Models.Entities;

namespace server.Models.Mappers;

public class BattleMapper
{
    public IEnumerable<BattleDto> ToDto(ICollection<Battle> battles)
    {
        List<BattleDto> battleDtos = new List<BattleDto>();
        foreach (var battle in battles)
        {
            var user = battle.BattleUsers.FirstOrDefault();
            user.User.Password = null;
            //user.User.BattleUsers = null;

            BattleDto battleDto = new BattleDto();
            battleDto.Id = battle.Id;
            battleDto.BattleStateId = battle.BattleStateId;
            battleDto.User = user.User;
            battleDto.UserId = user.Id;
            battleDto.TrackId = battle.TrackId;


            battleDtos.Add(battleDto);

        }
        return battleDtos;
    }

    public List<BattleDto> ToDtoWithAllInfo(ICollection<Battle> battles)
    {
        List<BattleDto> battleDtos = new List<BattleDto>();
        UserBattleMapper userBattleMapper = new UserBattleMapper();
        foreach (var battle in battles)
        {
            BattleDto battleDto = new BattleDto();
            battleDto.Id = battle.Id;
            battleDto.IsAgainstBot = battle.IsAgainstBot;
            battleDto.BattleStateId = battle.BattleStateId;
            battleDto.CreatedAt = battle.CreatedAt;
            battleDto.FinishedAt = battle.FinishedAt;
            battleDto.TrackId = battle.TrackId;
            battleDto.GameModeId = battle.GameModeId;

            var users = userBattleMapper.ToDto(battle.BattleUsers);
            battleDto.UsersBattles = users;

            battleDtos.Add(battleDto);

        }
        return battleDtos;
    }
}
