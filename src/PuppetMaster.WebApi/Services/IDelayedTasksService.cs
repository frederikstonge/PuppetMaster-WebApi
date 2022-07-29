using PuppetMaster.WebApi.Models.Database;

namespace PuppetMaster.WebApi.Services
{
    public interface IDelayedTasksService
    {
        void CancelTask(Guid roomId);

        void ScheduleCreateLobby(Guid matchId, Room room, TimeSpan delay);

        void SchedulePlayerPick(Guid userId, Guid matchId, Room room, TimeSpan delay);
    }
}