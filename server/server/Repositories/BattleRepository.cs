using Microsoft.EntityFrameworkCore;
using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class BattleRepository : Repository<Battle, int>
{
    public BattleRepository(Context context) : base(context) { }

    public async Task<Battle> GetBattleByUsersAsync(int userId1, int userId2)
    {
        return await GetQueryable()
            // Batallas no aceptadas o que se están jugando
            // Es decir, si hay una batalla que ya se está jugando o una petición de batalla, no se envía otra
            .Where(battle => battle.BattleUsers.Any(userBattle => userBattle.UserId == userId1))
            .Where(battle => battle.BattleUsers.Any(userBattle => userBattle.UserId == userId2))
            .Where(battle => battle.BattleStateId == 1 || battle.BattleStateId == 2 || battle.BattleStateId == 3)
            .FirstOrDefaultAsync();
    }

    public async Task<Battle> GetCompleteBattleAsync(int battleId)
    {
        return await GetQueryable()
            .Include(battle => battle.BattleUsers)
            .FirstOrDefaultAsync(battle => battle.Id == battleId);
    }

    public async Task<Battle> GetCompleteBattleWithUsersAsync(int battleId)
    {
        return await GetQueryable()
            .Include(battle => battle.BattleUsers)
            .ThenInclude(u => u.User)
            .FirstOrDefaultAsync(battle => battle.Id == battleId);
    }

    public async Task<ICollection<Battle>> GetBattlesInProgressAsync()
    {
        return await GetQueryable()
            .Where(b => b.BattleStateId == 3)
            .ToListAsync();

    }

    public async Task<ICollection<Battle>> GetPendingBattlesByUserIdAsync(int userId)
    {
        // Con el Any obtengo todas las batallas que incluyan al id del usuario :'D
        return await GetQueryable()
            .Where(battle => battle.BattleStateId == 1 || battle.BattleStateId == 2)
            .Where(battle => battle.BattleUsers.Any(user => user.UserId == userId && user.Receiver == true))
                .Include(battle => battle.BattleUsers.Where(user => user.UserId != userId))
                .ThenInclude(userBattle => userBattle.User)
                .ToListAsync();
    }

    public async Task<ICollection<Battle>> GetEndedBattlesByUserAsync(int userId)
    {
        return await GetQueryable()
            .Where(b => b.BattleStateId == 4)
            .Include(b => b.BattleUsers)
            .ThenInclude(ub => ub.User)
            .ToListAsync()
            .ContinueWith(task =>
                task.Result
                    .Where(b => b.BattleUsers.Any(ub => ub.UserId == userId))
                    .ToList()
            );
    }

    public async Task<Battle> GetBattleWithBotByUserAsync(int userId)
    {
        return await GetQueryable()
            .Where(battle => battle.BattleUsers.Any(user => user.UserId == userId))
            .FirstOrDefaultAsync(b => b.IsAgainstBot && b.BattleStateId == 3);
    }

    public async Task<ICollection<Battle>> GetCurrentBattleByUser(int userId)
    {
        return await GetQueryable()
            .Where(battle => battle.BattleStateId == 3)
            .Where(battle => battle.BattleUsers.Any(user => user.UserId == userId))
            .Include(battle => battle.BattleUsers)
            .ThenInclude(userBattle => userBattle.User)
            .ToListAsync();
    }

    public async Task<ICollection<Battle>> GetCurrentBattleByUserWithoutThem(int userId)
    {
        return await GetQueryable()
            .Where(battle => battle.BattleStateId == 3)
            .Where(battle => battle.BattleUsers.Any(user => user.UserId == userId))
            .Include(battle => battle.BattleUsers)
            .ToListAsync();
    }
}
