using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IGamesService
    {
        Task<Game> CreateGameAdminAsync(CreateGameAdminRequest request);

        Task<Game> GetGameAsync(Guid id);

        Task DeleteGameAdminAsync(Guid id);

        Task<List<Game>> GetGamesAsync();

        Task<Game> UpdateGameAdminAsync(Guid id, UpdateGameAdminRequest request);

        Task<Game> CreateGameMapAdminAsync(Guid id, CreateMapRequest request);

        Task<Game> UpdateGameMapAdminAsync(Guid id, Guid mapId, UpdateMapRequest request);

        Task<Game> DeleteGameMapAdminAsync(Guid id, Guid mapId);
    }
}