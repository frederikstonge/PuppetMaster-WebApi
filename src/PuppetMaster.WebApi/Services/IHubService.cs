using PuppetMaster.WebApi.Models.Database;

namespace PuppetMaster.WebApi.Services
{
    public interface IHubService
    {
        Task OnRoomChangedAsync(Room room);

        Task OnMatchChangedAsync(Match match, Room room);

        Task OnCreateLobbyAsync(Match match);

        Task OnSetupLobbyAsync(Match match);

        Task OnJoinLobbyAsync(Match match);

        Task OnMatchEndedAsync(Guid roomId);
    }
}