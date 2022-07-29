using System.Net;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Services
{
    public class MapsService : IMapsService
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public MapsService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Map> CreateGameMapAdminAsync(CreateMapRequest request)
        {
            var game = await _applicationDbContext.Games!.FirstOrDefaultAsync(g => g.Id == request.GameId);
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
            return map;
        }

        public async Task<Map> UpdateGameMapAdminAsync(Guid id, UpdateMapRequest request)
        {
            var map = await _applicationDbContext.Maps!.FirstOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            map.Name = request.Name;
            map.DisplayName = request.DisplayName;

            _applicationDbContext.Update(map);
            await _applicationDbContext.SaveChangesAsync();
            return map;
        }

        public async Task DeleteGameMapAdminAsync(Guid id)
        {
            var map = await _applicationDbContext.Maps!.FirstOrDefaultAsync(m => m.Id == id);
            if (map == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            _applicationDbContext.Remove(map);
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
