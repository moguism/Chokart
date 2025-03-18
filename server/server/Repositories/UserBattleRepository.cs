using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class UserBattleRepository : Repository<UserBattle, int>
{
    public UserBattleRepository(Context context) : base(context) { }
}

