using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class StateRepository : Repository<State, int>
{

    public StateRepository(Context context) : base(context) { }

}

