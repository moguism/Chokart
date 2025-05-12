using Microsoft.EntityFrameworkCore;
using server.Models.Entities;

namespace server;

public class Context : DbContext
{
    private const string DATABASE_PATH = "kartmario.db";

    public DbSet<Battle> Battles { get; set; }
    public DbSet<BattleState> BattleStates { get; set; }
    public DbSet<BattleResult> BattleResults { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserBattle> UsersBattles { get; set; }
    public DbSet<Character> Characters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
# if DEBUG
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        optionsBuilder.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");

        //string key = "Server=db16516.public.databaseasp.net; Database=db16516; Uid=db16516; Pwd=6Ct?k5%G+A2w; SslMode=Preferred; ";
        //optionsBuilder.UseMySql(key, ServerVersion.AutoDetect(key));
#else
string key = "Server=db16516.public.databaseasp.net; Database=db16516; Uid=db16516; Pwd=6Ct?k5%G+A2w; SslMode=Preferred; ";
        optionsBuilder.UseMySql(key, ServerVersion.AutoDetect(key));
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.SenderUser)
            .WithMany(u => u.Friendships)
            .HasForeignKey(f => f.SenderUserId);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.ReceiverUser)
            .WithMany()
            .HasForeignKey(f => f.ReceiverUserId);
    }
}
