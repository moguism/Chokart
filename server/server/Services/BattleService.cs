using server.Models.DTOs;
using server.Models.Entities;
using server.Models.Mappers;
using server.Repositories;

namespace server.Services;

public class BattleService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly BattleService battleService;

    public BattleService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateBattleAsync(BattlePetition battlePetition)
    {
        try
        {
            Battle battle = new Battle()
            {
                GameModeId = battlePetition.GameMode,
                TrackId = battlePetition.TrackId
            };

            foreach (FinishKart finishKart in battlePetition.FinishKarts)
            {
                // No guardo a los jugadores que sean un bot
                if (finishKart.PlayerId == 0)
                {
                    continue;
                }

                UserBattle userBattle = new UserBattle()
                {
                    UserId = finishKart.PlayerId,
                    TotalKills = finishKart.Kills,
                    Position = finishKart.Position,
                    CharacterId = finishKart.CharacterId
                };

                User player = await _unitOfWork.UserRepository.GetUserById(finishKart.PlayerId);
                player.TotalPoints = finishKart.Kills + (battlePetition.FinishKarts.Count() - finishKart.Position)+1;
                _unitOfWork.UserRepository.Update(player);

                battle.BattleUsers.Add(userBattle);
            }

            await _unitOfWork.BattleRepository.InsertAsync(battle);
            await _unitOfWork.SaveAsync();
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<ICollection<Battle>> GetEndedBattlesByUserAsync(int userId)
    {
        return await _unitOfWork.BattleRepository.GetEndedBattlesByUserAsync(userId);
    }


}
