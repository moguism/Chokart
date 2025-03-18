
using server.Models.Entities;
using server.Repositories;
using System.Threading.Tasks;

namespace server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<Context>();
            builder.Services.AddScoped<UnitOfWork>();

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(
                options =>
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                        ;
                    })
                );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseWebSockets();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseStaticFiles();

            await SeedDataBaseAsync(app.Services);

            app.Run();
        }

        static async Task SeedDataBaseAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            using Context dbContext = scope.ServiceProvider.GetService<Context>();

            // Si no existe la base de datos, la creamos y ejecutamos el seeder
            if (dbContext.Database.EnsureCreated())
            {
                //Seeder seeder = new Seeder(dbContext);
                //await seeder.SeedAsync();
            }

            // Por si se va la luz 😎
            UnitOfWork unitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();
            ICollection<Battle> battles = await unitOfWork.BattleRepository.GetBattlesInProgressAsync();
            foreach (Battle battle in battles)
            {
                unitOfWork.BattleRepository.Delete(battle);
            }
            await unitOfWork.SaveAsync();
        }
    }
}
