namespace PuppetMaster.WebApi.Services
{
    public interface IDelayedTasksService
    {
        void CancelTask(Guid roomId);

        void SchedulePlayerPick(Guid userId, Guid matchId, Guid roomId, TimeSpan delay);

        void ScheduleCreateLobby(Guid matchId, Guid roomId, TimeSpan delay);

        void HasJoinedTimeout(Guid matchId, Guid roomId, TimeSpan delay);
    }
}