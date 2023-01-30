using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Moq;
using PuppetMaster.WebApi.Hubs;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Profiles;
using PuppetMaster.WebApi.Repositories;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Tests
{
    public class IntegrationTests : MoqBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly GamesService _gamesService;
        private readonly MapsService _mapsService;
        private readonly GameUsersService _gameUsersService;
        private readonly HubService _hubService;
        private readonly RoomsService _roomsService;
        private readonly MatchesService _matchesService;
        private readonly Mock<IDelayedTasksService> _delayedTasksServiceMock;
        private Mock<IHubContext<RoomHub>> _hubContextMock;

        public IntegrationTests()
        {
            _dbContext = DbContextHelper.GetInMemoryDbContext();

            _hubContextMock = MockRepository.Create<IHubContext<RoomHub>>();
            _delayedTasksServiceMock = MockRepository.Create<IDelayedTasksService>();

            _gamesService = new GamesService(_dbContext);
            _mapsService = new MapsService(_dbContext);
            _gameUsersService = new GameUsersService(_dbContext);
            _hubService = new HubService(GetMapper(), _hubContextMock.Object, _delayedTasksServiceMock.Object);
            _roomsService = new RoomsService(_dbContext, _hubService);
            _matchesService = new MatchesService(_dbContext, _hubService);
        }

        public override void Dispose()
        {
            _dbContext.Dispose();
            base.Dispose();
        }

        [Fact]
        public async Task IntegrationTest()
        {
            // Create game and maps
            var game = await CreateGamesAsync();

            // Create users
            var users = new List<ApplicationUser>();
            for (int i = 0; i < 10; i++)
            {
                users.Add(await CreateUserAsync($"test{i}@test.com", $"test{i}"));
            }

            // Create game users
            foreach (var user in users)
            {
                await _gameUsersService.CreateGameUserAdminAsync(new Models.Requests.CreateGameUserAdminRequest()
                {
                    ApplicationUserId = user.Id,
                    GameId = game.Id,
                    Region = "na",
                    UniqueGameId = user.Id.ToString()
                });
            }

            var firstUser = users.First();

            // Create room
            var room = await _roomsService.CreateRoomAsync(firstUser.Id, new Models.Requests.CreateRoomRequest()
            {
                GameId = game.Id,
                Name = "test"
            });

            // Setup HubContext
            _hubContextMock
                .Setup(c => c.Clients.Group(It.IsAny<string>()).SendCoreAsync(SignalRMethods.RoomChanged, It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            _hubContextMock
                .Setup(c => c.Clients.Group(It.IsAny<string>()).SendCoreAsync(SignalRMethods.MatchChanged, It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            _hubContextMock
                .Setup(c => c.Clients.User(It.IsAny<string>()).SendCoreAsync(SignalRMethods.CreateLobby, It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            _hubContextMock
                .Setup(c => c.Clients.User(It.IsAny<string>()).SendCoreAsync(SignalRMethods.JoinLobby, It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            _hubContextMock
                .Setup(c => c.Clients.User(It.IsAny<string>()).SendCoreAsync(SignalRMethods.SetupLobby, It.IsAny<object[]>(), default))
                .Returns(Task.CompletedTask);

            // Setup DelayedTaskService
            _delayedTasksServiceMock.Setup(s => s.CancelTask(It.IsAny<Guid>()));
            _delayedTasksServiceMock.Setup(s => s.ScheduleCreateLobby(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()));
            _delayedTasksServiceMock.Setup(s => s.SchedulePlayerPick(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()));
            _delayedTasksServiceMock.Setup(s => s.HasJoinedTimeout(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TimeSpan>()));

            var joinCount = room.RoomUsers!.Count;

            // Join room
            foreach (var user in users.Where(u => u.Id != firstUser.Id))
            {
                Assert.Equal(joinCount++, room.RoomUsers!.Count);
                await _roomsService.JoinRoomAsync(user.Id, room.Id, null);
            }

            var readyCount = room.RoomUsers!.Where(ru => ru.IsReady).Count();

            // Ready up
            foreach (var user in users)
            {
                Assert.Equal(readyCount++, room.RoomUsers!.Where(ru => ru.IsReady).Count());
                await Task.Delay(1000);
                await _roomsService.ReadyRoomAsync(user.Id, room.Id, true);
            }

            // Create match
            var match = await _matchesService.CreateMatchAsync(room.Id);

            // Pick players
            var pickedPlayers = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!
                .Select(mtu => mtu.ApplicationUserId))
                .ToList();

            var availablePlayers = match.Users!
                    .ExceptBy(pickedPlayers, u => u!.Id)
                    .ToList();

            do
            {
                var teamTurn = match.MatchTeams!
                    .OrderBy(mt => mt.MatchTeamUsers!.Count)
                    .ThenBy(mt => mt.TeamIndex)
                    .First();

                var captainToPickThisTurn = teamTurn.MatchTeamUsers!
                    .Single(mtu => mtu.IsCaptain)
                    .ApplicationUserId;

                var pickedPlayer = availablePlayers.First();

                await _matchesService.PickPlayerAsync(captainToPickThisTurn, match.Id, pickedPlayer.Id);
                availablePlayers.Remove(pickedPlayer);
                await Task.Delay(1000);
            }
            while (availablePlayers.Any());

            Assert.Equal(match.Users!.Count, match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).Count());

            // Vote Map
            foreach (var user in users)
            {
                await _matchesService.VoteMapAsync(user.Id, match!.Id, game.Maps!.First().Name);
                await Task.Delay(1000);
            }

            Assert.Equal(
                match.Users.Count, 
                match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).Select(mtu => mtu.MapVote == game.Maps!.First().Name).Count());

            // Create Lobby
            await _matchesService.SetLobbyIdAsync(match.LobbyLeaderId!.Value, match.Id, "lobbyId");
            await Task.Delay(1000);

            // Join Lobby
            foreach (var user in users)
            {
                await _matchesService.HasJoinedAsync(user.Id, match.Id);
                await Task.Delay(1000);
            }
        }

        private async Task<ApplicationUser> CreateUserAsync(string email, string userName)
        {
            var user = new ApplicationUser()
            {
                Email = email,
                FirstName = email,
                LastName = email,
                UserName = userName,
                EmailConfirmed = true,
                NormalizedEmail = email,
                NormalizedUserName = userName
            };
            _dbContext.Users.Add(user);

            await _dbContext.SaveChangesAsync();
            return user;
        }

        private async Task<Game> CreateGamesAsync()
        {
            var games = await _gamesService.GetGamesAsync();
            var game = games.FirstOrDefault(g => g.Name == "Valorant");
            if (game == null)
            {
                game = await _gamesService.CreateGameAdminAsync(new Models.Requests.CreateGameAdminRequest()
                {
                    Name = "Valorant",
                    IconUrl = "/images/valorant.jpg",
                    PlayerCount = 10,
                    TeamCount = 2
                });
            }

            await CreateMapForGame(game, "Ascent", "Ascent");
            await CreateMapForGame(game, "Duality", "Bind");
            await CreateMapForGame(game, "Bonsai", "Split");
            await CreateMapForGame(game, "Triad", "Haven");
            await CreateMapForGame(game, "Port", "Icebox");
            await CreateMapForGame(game, "Foxtrot", "Breeze");
            await CreateMapForGame(game, "Canyon", "Fracture");
            await CreateMapForGame(game, "Pitt", "Pearl");
            await CreateMapForGame(game, "Jam", "Lotus");

            return game;
        }

        private async Task CreateMapForGame(Game game, string name, string displayName)
        {
            var maps = await _mapsService.GetMapsAsync(game.Id);
            if (!maps.Any(m => m.Name == name && m.DisplayName == displayName))
            {
                await _mapsService.CreateGameMapAdminAsync(
                    new Models.Requests.CreateMapRequest()
                    {
                        DisplayName = displayName,
                        Name = name,
                        GameId = game.Id
                    });
            }
        }

        private IMapper GetMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new GameProfile());
                cfg.AddProfile(new GameUserProfile());
                cfg.AddProfile(new MapProfile());
                cfg.AddProfile(new MatchProfile());
                cfg.AddProfile(new MatchTeamProfile());
                cfg.AddProfile(new MatchTeamUserProfile());
                cfg.AddProfile(new RoomProfile());
                cfg.AddProfile(new RoomUserProfile());
                cfg.AddProfile(new UserProfile());
            });

            return new Mapper(configuration);
        }
    }
}
