using PuppetMaster.WebApi.Models.Database;

namespace PuppetMaster.WebApi.Services
{
    public interface IDelayedTasksService
    {
        void CancelTask(Guid roomId);

        void ScheduleCreateLobby(Guid matchId, Guid roomId, TimeSpan delay);

        void SchedulePlayerPick(Guid userId, Guid matchId, Guid roomId, TimeSpan delay);
    }
}