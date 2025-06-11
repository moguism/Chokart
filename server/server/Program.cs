
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using server.Models.Mappers;
using server.Repositories;
using server.Services;
using server.Sockets;

namespace server
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppContext.BaseDirectory);

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<Context>();
            builder.Services.AddScoped<UnitOfWork>();
            builder.Services.AddScoped<UserMapper>();
            builder.Services.AddScoped<BattleMapper>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<BattleService>();
            builder.Services.AddScoped<FriendshipService>();
            builder.Services.AddScoped<SteamService>();

            builder.Services.AddSingleton<WebSocketHandler>();

            builder.Services.AddTransient<PreAuthMiddleware>();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // CONFIGURANDO JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    string key = Environment.GetEnvironmentVariable("JwtKey");
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                    };
                })
                .AddCookie("SteamCookie") // Este es el valor que necesita el navegador para ubicarse
                .AddSteam("Steam", options =>
                {
                    options.ApplicationKey = Environment.GetEnvironmentVariable("STEAM_KEY");
                    options.SignInScheme = "SteamCookie";
                    options.CallbackPath = "/api/SteamAuth/steam-callback";
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseHttpsRedirection();
            app.UseWebSockets();
            app.UseMiddleware<PreAuthMiddleware>();


            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            app.MapControllers();

            app.Map("api/{**slug}", HandleApiFallbackAsync);
            app.MapFallbackToFile("client/index.html");

            await SeedDataBaseAsync(app.Services);

            app.Run();
        }

        static async Task SeedDataBaseAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            using Context dbContext = scope.ServiceProvider.GetService<Context>();

            UnitOfWork unitOfWork = scope.ServiceProvider.GetService<UnitOfWork>();

            // Si no existe la base de datos, la creamos y ejecutamos el seeder
            if (dbContext.Database.EnsureCreated())
            {
                Seeder seeder = new Seeder(dbContext);
                await seeder.SeedAsync();
            }
        }

        private static IResult HandleApiFallbackAsync(HttpContext context)
        {
            return Results.NotFound($"Cannot {context.Request.Method}{context.Request.Path}");
        }
    }
}
