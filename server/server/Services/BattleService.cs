using server.Models.DTOs;
using server.Models.Entities;
using server.Models.Mappers;
using server.Repositories;
using System.Numerics;

namespace server.Services;

public class BattleService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly BattleMapper _battleMapper;

    public BattleService(UnitOfWork unitOfWork, BattleMapper battleMapper)
    {
        _unitOfWork = unitOfWork;
        _battleMapper = battleMapper;
    }

    public async Task CreateBattleAsync(BattlePetition battlePetition, int actualUserId)
    {
        try
        {
            var actualUser = await _unitOfWork.UserRepository.GetUserByIdNoTraking(actualUserId);

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

                int points = finishKart.Kills + (battlePetition.FinishKarts.Count() - finishKart.Position) + 1;

                if (finishKart.PlayerId == actualUser.Id)
                {
                    actualUser.TotalPoints += points;
                    _unitOfWork.UserRepository.Update(actualUser);
                }
                else
                {
                    var player = await _unitOfWork.UserRepository.GetUserByIdNoTraking(finishKart.PlayerId);

                    if (player != null)
                    {
                        player.TotalPoints += points;
                        _unitOfWork.UserRepository.Update(player);
                    }
                }

                battle.BattleUsers.Add(userBattle);
            }

            await _unitOfWork.BattleRepository.InsertAsync(battle);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }


    public async Task<ICollection<BattleDto>> GetEndedBattlesByUserAsync(int userId)
    {
        var battles = await _unitOfWork.BattleRepository.GetEndedBattlesByUserAsync(userId);
        var battleDtos = _battleMapper.ToDtoWithAllInfo(battles); 
        return battleDtos;
    }


}
