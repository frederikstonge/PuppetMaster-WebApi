using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IRoomsService
    {
        Task<Room> CreateRoomAsync(Guid userId, CreateRoomRequest request);

        Task<List<Room>> GetRoomsAsync(Guid? gameId, string? region);

        Task<Room> GetRoomAsync(Guid id);

        Task<Room> JoinRoomAsync(Guid userId, Guid id, string? password);

        Task<Room?> LeaveRoomAsync(Guid userId, Guid id);

        Task<Room> ReadyRoomAsync(Guid userId, Guid id, bool isReady);
    }
}