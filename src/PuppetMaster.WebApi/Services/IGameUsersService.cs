using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IGameUsersService
    {
        Task<GameUser> CreateGameUserAsync(Guid userId, CreateGameUserRequest request);

        Task<GameUser> GetGameUserAsync(Guid userId, Guid id);

        Task<List<GameUser>> GetGameUsersAsync(Guid userId);

        Task<GameUser> UpdateGameUserAsync(Guid userId, Guid id, UpdateGameUserRequest request);

        Task<GameUser> CreateGameUserAdminAsync(CreateGameUserAdminRequest request);

        Task<GameUser> UpdateGameUserAdminAsync(Guid id, UpdateGameUserRequest request);

        Task DeleteGameUserAdminAsync(Guid id);
    }
}