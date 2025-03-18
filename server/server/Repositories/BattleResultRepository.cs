using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class BattleResultRepository : Repository<BattleResult, int>
{
    public BattleResultRepository(Context context) : base(context) { }
}
