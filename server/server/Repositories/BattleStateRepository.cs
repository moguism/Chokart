using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class BattleStateRepository : Repository<BattleState, int>
{
    public BattleStateRepository(Context context) : base(context) { }
}
