using System.Net;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Services
{
    public class GameUsersService : IGameUsersService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public GameUsersService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<List<GameUser>> GetGameUsersAsync(Guid userId)
        {
            var applicationUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == applicationUser.Id)
                .Include(gu => gu.Game!)
                .ThenInclude(g => g.Maps!)
                .ToListAsync();
        }

        public async Task<GameUser> GetGameUserAsync(Guid userId, Guid id)
        {
            var applicationUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == applicationUser.Id && gu.Id == id)
                .Include(gu => gu.Game!)
                .ThenInclude(g => g.Maps!)
                .FirstOrDefaultAsync();

            if (gameUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return gameUser;
        }

        public async Task<GameUser> CreateGameUserAsync(Guid userId, CreateGameUserRequest request)
        {
            var applicationUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var game = await _applicationDbContext.Games!.Where(g => g.Id == request.GameId).FirstOrDefaultAsync();
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == applicationUser.Id && gu.UniqueGameId == request.UniqueGameId && gu.GameId == request.GameId)
                .FirstOrDefaultAsync();

            if (gameUser != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            gameUser = new GameUser()
            {
                UniqueGameId = request.UniqueGameId,
                ApplicationUserId = applicationUser.Id,
                GameId = request.GameId,
                Region = request.Region
            };

            await _applicationDbContext.AddAsync(gameUser);
            await _applicationDbContext.SaveChangesAsync();
            return gameUser;
        }

        public async Task<GameUser> UpdateGameUserAsync(Guid userId, Guid id, UpdateGameUserRequest request)
        {
            var applicationUser = await _applicationDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (applicationUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == applicationUser.Id && gu.Id == id)
                .FirstOrDefaultAsync();

            if (gameUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            if (gameUser.ModifiedDate < DateTime.Now.AddMonths(-2))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            gameUser.UniqueGameId = request.UniqueGameId;
            gameUser.Region = request.Region;
            _applicationDbContext.Update(gameUser);
            await _applicationDbContext.SaveChangesAsync();
            return gameUser;
        }

        public async Task<GameUser> CreateGameUserAdminAsync(CreateGameUserAdminRequest request)
        {
            var game = await _applicationDbContext.Games!.Where(g => g.Id == request.GameId).FirstOrDefaultAsync();
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == request.ApplicationUserId && gu.UniqueGameId == request.UniqueGameId && gu.GameId == request.GameId)
                .FirstOrDefaultAsync();

            if (gameUser != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            gameUser = new GameUser()
            {
                UniqueGameId = request.UniqueGameId,
                ApplicationUserId = request.ApplicationUserId,
                GameId = request.GameId,
                Region = request.Region
            };

            await _applicationDbContext.AddAsync(gameUser);
            await _applicationDbContext.SaveChangesAsync();
            return gameUser;
        }

        public async Task<GameUser> UpdateGameUserAdminAsync(Guid id, UpdateGameUserRequest request)
        {
            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.Id == id)
                .FirstOrDefaultAsync();

            if (gameUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            gameUser.UniqueGameId = request.UniqueGameId;
            gameUser.Region = request.Region;
            _applicationDbContext.Update(gameUser);
            await _applicationDbContext.SaveChangesAsync();
            return gameUser;
        }

        public async Task DeleteGameUserAdminAsync(Guid id)
        {
            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.Id == id)
                .FirstOrDefaultAsync();

            if (gameUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _applicationDbContext.Remove(gameUser);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
