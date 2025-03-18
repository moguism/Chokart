using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories;

public class TrackRepository : Repository<Track, int>
{
    public TrackRepository(Context context) : base(context) { }
}

