using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class KartRepository : Repository<Kart, int>
{
    public KartRepository(Context context) : base(context) { }
}

