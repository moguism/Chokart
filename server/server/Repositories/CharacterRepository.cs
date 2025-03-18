using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class CharacterRepository : Repository<Character, int>
{
    public CharacterRepository(Context context) : base(context) { }
}

