using Microsoft.EntityFrameworkCore;
using server.Models.Entities;
using server.Repositories.Base;

namespace server.Repositories
{
    public class FriendshipRepository : Repository<Friendship, int>
    {
        public FriendshipRepository(Context context) : base(context) { }
        public async Task<Friendship> GetFriendshipAsync(int userId1, int userId2)
        {
            return await GetQueryable()
                .Include(friendship => friendship.SenderUser)
                .Include(f => f.ReceiverUser)
                .FirstOrDefaultAsync(f => f.SenderUserId == userId1 && f.ReceiverUserId == userId2);

        }

        /*public async Task<Friendship> GetFriendshipByIdAsync(int id)
        {
            return await GetQueryable()
                .Include(friendship => friendship.SenderUser)
                .Include(f => f.ReceiverUser)
                .FirstOrDefaultAsync(f => f.Id == id);

        }*/

        public async Task<ICollection<Friendship>> GetFriendshipsByUserAsync(int userId)
        {
            return await GetQueryable()
                .Include(friendship => friendship.SenderUser)
                    .ThenInclude(u => u.State)
                .Include(f => f.ReceiverUser)
                    .ThenInclude(u => u.State)
                .Where(f => f.SenderUserId == userId || f.ReceiverUserId == userId)
                .ToListAsync();
        }
    }
}
