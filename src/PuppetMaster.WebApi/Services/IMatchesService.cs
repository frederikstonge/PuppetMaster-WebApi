using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IMatchesService
    {
        Task<Match> CreateMatchAsync(Guid roomId);

        Task<Match> GetMatchAsync(Guid id);

        Task<Match> HasJoinedAsync(Guid userId, Guid id);

        Task<Match> PickPlayerAsync(Guid userId, Guid id, Guid pickedUserId);

        Task<Match> SetLobbyIdAsync(Guid userId, Guid id, string lobbyId);

        Task<Match> VoteMapAsync(Guid userId, Guid id, string voteMap);

        Task<Match> MatchEndedAsync(Guid userId, Guid id, MatchEndedRequest request);

        Task<Match> MatchAbandonnedAsync(Guid id);
    }
}