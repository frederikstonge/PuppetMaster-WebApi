using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Repositories;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi
{
    public class DataWorker : IHostedService
    {
        private const string DefaultUserName = "admin";
        private const string DefaultEmail = "admin@gmail.com";
        private const string DefaultFirstName = "admin";
        private const string DefaultLastName = "admin";
        private const string DefaultPassword = "Test123!";

        private readonly IServiceProvider _serviceProvider;

        public DataWorker(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            await EnsureDatabaseAsync(scope);
            await CreateAdminUserAsync(scope);
            await CreateGamesAsync(scope);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static Task EnsureDatabaseAsync(IServiceScope scope)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return dbContext.Database.MigrateAsync();
        }

        private static async Task CreateAdminUserAsync(IServiceScope scope)
        {
            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = dbContext.Users.FirstOrDefault(u => u.UserName == DefaultUserName || u.Email == DefaultEmail);
            if (user == null)
            {
                try
                {
                    await accountService.RegisterAsync(new Models.Requests.RegisterRequest()
                    {
                        Email = DefaultEmail,
                        Password = DefaultPassword,
                        UserName = DefaultUserName,
                        FirstName = DefaultFirstName,
                        LastName = DefaultLastName
                    });
                }
                catch (HttpResponseException ex)
                {
                    if (ex.StatusCode != System.Net.HttpStatusCode.Conflict)
                    {
                        throw;
                    }
                }

                user = dbContext.Users.First(u => u.UserName == DefaultUserName && u.Email == DefaultEmail);
                await accountService.AddToRoleAsync(user, Role.Administrator);
            }
        }

        private static async Task<Models.Database.Game> CreateGamesAsync(IServiceScope scope)
        {
            var gameService = scope.ServiceProvider.GetRequiredService<IGamesService>();
            var games = await gameService.GetGamesAsync();
            var game = games.FirstOrDefault(g => g.Name == "Valorant");
            if (game == null)
            {
                game = await gameService.CreateGameAdminAsync(new Models.Requests.CreateGameAdminRequest()
                {
                    Name = "Valorant",
                    IconUrl = "/images/valorant.jpg",
                    PlayerCount = 10,
                    TeamCount = 2
                });
            }

            await CreateMapForGame(scope, game, "Ascent", "Ascent");
            await CreateMapForGame(scope, game, "Duality", "Bind");
            await CreateMapForGame(scope, game, "Bonsai", "Split");
            await CreateMapForGame(scope, game, "Triad", "Haven");
            await CreateMapForGame(scope, game, "Port", "Icebox");
            await CreateMapForGame(scope, game, "Foxtrot", "Breeze");
            await CreateMapForGame(scope, game, "Canyon", "Fracture");
            await CreateMapForGame(scope, game, "Pitt", "Pearl");
            await CreateMapForGame(scope, game, "Jam", "Lotus");

            return game;
        }

        private static async Task CreateMapForGame(IServiceScope scope, Models.Database.Game game, string name, string displayName)
        {
            var mapService = scope.ServiceProvider.GetRequiredService<IMapsService>();
            var maps = await mapService.GetMapsAsync(game.Id);
            if (!maps.Any(m => m.Name == name && m.DisplayName == displayName))
            {
                await mapService.CreateGameMapAdminAsync(
                    new Models.Requests.CreateMapRequest() 
                    { 
                        DisplayName = displayName, 
                        Name = name,
                        GameId = game.Id 
                    });
            }
        }
    }
}
