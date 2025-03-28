
using Microsoft.IdentityModel.Tokens;
using server.Models.Entities;
using server.Models.Mappers;
using server.Repositories;
using server.Services;
using server.Sockets;
using server.Sockets.Game;
using System.Text;
using System.Text.Json.Serialization;

namespace server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<Context>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<UserMapper>();
            builder.Services.AddScoped<UserService>();

            builder.Services.AddSingleton<WebSocketHandler>();
            builder.Services.AddSingleton<GameNetwork>();

            builder.Services.AddTransient<PreAuthMiddleware>();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // CONFIGURANDO JWT
            builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    string key = Environment.GetEnvironmentVariable("JwtKey");
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,

                        // INDICAMOS LA CLAVE
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                });

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
            app.UseMiddleware<PreAuthMiddleware>();

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
                Seeder seeder = new Seeder(dbContext);
                await seeder.SeedAsync();
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
