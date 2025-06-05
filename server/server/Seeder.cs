using server.Models.Entities;
using server.Models.Helper;

namespace server
{
    public class Seeder
    {
        private readonly Context _context;

        private User[] users = new User[2];

        public Seeder(Context context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            string defaultPassword = PasswordHelper.Hash(Environment.GetEnvironmentVariable("DefaultPassword"));
            users = [
                new User
                {
                    Nickname = "moguism",
                    Email = "a@a.com",
                    Password = defaultPassword,
                    Role = "Admin",
                    StateId = 1,
                    Verified = true,
                    VerificationCode = "MONDONGO"
                },
                new User
                {
                    Nickname = "zero",
                    Email = "maria@gmail.com",
                    Password = defaultPassword,
                    Role = "Admin",
                    StateId = 1,
                    Verified = true,
                    VerificationCode = "MONDONGO"
                }
            ];

            await SeedStateAsync();
            await SeedUsersAsync();
            await SeedBattleResultAsync();
            await SeedBattleStateAsync();
            await SeedCharactersAsync();
            await SeedTracksAsync();
            await SeedFriendshipsAsync();
            await _context.SaveChangesAsync();
        }
        private async Task SeedStateAsync()
        {
            State[] states = [
                    new State {
                    Name = "Desconectado"
                },
                new State {
                    Name = "Conectado"
                },
                new State {
                    Name = "Jugando"
                }
                ];

            await _context.States.AddRangeAsync(states);
        }

        private async Task SeedBattleResultAsync()
        {
            BattleResult[] results = [
                new BattleResult()
                {
                    Name = "Victoria"
                }
            ];

            await _context.BattleResults.AddRangeAsync(results);
        }

        private async Task SeedBattleStateAsync()
        {
            BattleState[] states = [
                new BattleState()
                {
                    Name = "Fin"
                }
            ];

            await _context.BattleStates.AddRangeAsync(states);
        }

        private async Task SeedFriendshipsAsync()
        {
            Friendship[] friendships = [
                new Friendship()
                {
                    Accepted = true,
                    ReceiverUserId = 1,
                    ReceiverUser = users[0],
                    SenderUserId = 2,
                    SenderUser = users[1]
                }
            ];

            await _context.Friendships.AddRangeAsync(friendships);
        }

        private async Task SeedCharactersAsync()
        {
            Character[] characters = [
                new Character()
                {
                    Name = "Jinx"
                },
                new Character()
                {
                    Name = "Shrek"
                },
                new Character()
                {
                    Name = "Alastor"
                },
                new Character()
                {
                    Name = "Pingu"
                },
                new Character()
                {
                    Name = "Lefty"
                },
                new Character()
                {
                    Name = "Spamton"
                }
            ];

            await _context.Characters.AddRangeAsync(characters);
        }

        private async Task SeedTracksAsync()
        {
            Track[] tracks = [
                new Track()
                {
                    Name = "Shy Guy Bazaar"
                }
            ];

            await _context.Tracks.AddRangeAsync(tracks);
        }

        private async Task SeedUsersAsync()
        {
            await _context.Users.AddRangeAsync(users);
        }
    }
}
