using server.Models.DTOs;
using server.Models.Entities;
using server.Repositories;

namespace server.Services;

public class BattleService
{
    private readonly UnitOfWork _unitOfWork;

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
                GameModeId = battlePetition.GameMode
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
                };

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
}
