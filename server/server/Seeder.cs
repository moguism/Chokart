using server.Models.Entities;
using server.Models.Helper;

namespace server
{
    public class Seeder
    {
        private readonly Context _context;

        public Seeder(Context context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await SeedStateAsync();
            await SeedUsersAsync();
            await SeedBattleResultAsync();
            await SeedBattleStateAsync();
            await SeedCharactersAsync();
            await SeedTracksAsync();
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

        private async Task SeedCharactersAsync()
        {
            Character[] characters = [
                new Character()
                {
                    Name = "Jinx"
                }
            ];

            await _context.Characters.AddRangeAsync(characters);
        }

        private async Task SeedTracksAsync()
        {
            Track[] tracks = [
                new Track()
                {
                    Name = "Moo moo farm"
                }
            ];

            await _context.Tracks.AddRangeAsync(tracks);
        }

        private async Task SeedUsersAsync()
        {
            string defaultPassword = PasswordHelper.Hash(Environment.GetEnvironmentVariable("DefaultPassword"));
            User[] users = [
                new User
                {
                    Nickname = "moguism",
                    Email = "a@a.com",
                    Password = defaultPassword,
                    Role = "Admin",
                    StateId = 1
                },
                new User
                {
                    Nickname = "zero",
                    Email = "maria@gmail.com",
                    Password = defaultPassword,
                    Role = "Admin",
                    StateId = 1
                }
            ];

            await _context.Users.AddRangeAsync(users);
        }
    }
}
