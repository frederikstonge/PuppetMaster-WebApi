using System.Net;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Services
{
    public class GamesService : IGamesService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GamesService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Task<List<Game>> GetGamesAsync()
        {
            return _applicationDbContext.Games!.Include(g => g.Maps).ToListAsync();
        }

        public async Task<Game> GetGameAsync(Guid id)
        {
            var game = await _applicationDbContext.Games!
                .Include(g => g.Maps!)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return game;
        }

        public async Task<Game> CreateGameAdminAsync(CreateGameAdminRequest request)
        {
            if (_applicationDbContext.Games!.Any(g => g.Name == request.Name))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var game = new Game()
            {
                Name = request.Name,
                IconUrl = request.IconUrl,
                PlayerCount = request.PlayerCount,
                TeamCount = request.TeamCount,
            };

            await _applicationDbContext.AddAsync(game);
            await _applicationDbContext.SaveChangesAsync();

            return game;
        }

        public async Task DeleteGameAdminAsync(Guid id)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _applicationDbContext.Games!.Remove(game);
            await _applicationDbContext.SaveChangesAsync();
        }

        public async Task<Game> UpdateGameAdminAsync(Guid id, UpdateGameAdminRequest request)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            game.Name = request.Name;
            game.IconUrl = request.IconUrl;
            game.TeamCount = request.TeamCount;
            game.PlayerCount = request.PlayerCount;

            _applicationDbContext.Update(game);
            await _applicationDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<Game> CreateGameMapAdminAsync(Guid id, CreateMapRequest request)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var map = new Map()
            {
                GameId = game.Id,
                Name = request.Name,
                DisplayName = request.DisplayName
            };

            await _applicationDbContext.AddAsync(map);
            await _applicationDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<Game> UpdateGameMapAdminAsync(Guid id, Guid mapId, UpdateMapRequest request)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var map = await _applicationDbContext.Maps!.FirstOrDefaultAsync(m => m.Id == mapId && m.GameId == id);
            if (map == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            map.Name = request.Name;
            map.DisplayName = request.DisplayName;

            _applicationDbContext.Update(map);
            await _applicationDbContext.SaveChangesAsync();
            return game;
        }

        public async Task<Game> DeleteGameMapAdminAsync(Guid id, Guid mapId)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var map = await _applicationDbContext.Maps!.FirstOrDefaultAsync(m => m.Id == mapId && m.GameId == id);
            if (map == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _applicationDbContext.Remove(map);
            await _applicationDbContext.SaveChangesAsync();
            return game;
        }
    }
}
