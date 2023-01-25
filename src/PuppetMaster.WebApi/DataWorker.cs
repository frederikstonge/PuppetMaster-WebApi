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

        private static async Task CreateGamesAsync(IServiceScope scope)
        {
            var gameService = scope.ServiceProvider.GetRequiredService<IGamesService>();
            var mapService = scope.ServiceProvider.GetRequiredService<IMapsService>();
            var games = await gameService.GetGamesAsync();
            if (!games.Any(g => g.Name == "Valorant"))
            {
                var game = await gameService.CreateGameAdminAsync(new Models.Requests.CreateGameAdminRequest()
                {
                    Name = "Valorant",
                    IconUrl = "/images/valorant.jpg",
                    PlayerCount = 10,
                    TeamCount = 2
                });

                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Ascent", Name = "Ascent", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Bind", Name = "Duality", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Split", Name = "Bonsai", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Haven", Name = "Triad", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Icebox", Name = "Port", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Breeze", Name = "Foxtrot", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Fracture", Name = "Canyon", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Pearl", Name = "Pitt", GameId = game.Id });
                await mapService.CreateGameMapAdminAsync(new Models.Requests.CreateMapRequest() { DisplayName = "Lotus", Name = "Jam", GameId = game.Id });
            }
        }
    }
}
