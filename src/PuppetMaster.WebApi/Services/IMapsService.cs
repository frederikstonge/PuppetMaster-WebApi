using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IMapsService
    {
        Task<Map> CreateGameMapAdminAsync(CreateMapRequest request);

        Task DeleteGameMapAdminAsync(Guid id);

        Task<Map> UpdateGameMapAdminAsync(Guid id, UpdateMapRequest request);
    }
}