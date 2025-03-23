using Microsoft.EntityFrameworkCore;
using server.Models.Entities;

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
    }
}
