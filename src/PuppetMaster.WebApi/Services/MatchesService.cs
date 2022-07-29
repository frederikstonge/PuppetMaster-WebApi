using System.Net;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Services
{
    public class MatchesService : IMatchesService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHubService _hubService;
        private readonly IDelayedTasksService _delayedTasksService;

        public MatchesService(ApplicationDbContext applicationDbContext, IHubService hubService, IDelayedTasksService delayedTasksService)
        {
            _applicationDbContext = applicationDbContext;
            _hubService = hubService;
            _delayedTasksService = delayedTasksService;
        }

        public Task<Match> GetMatchAsync(Guid id)
        {
            return GetMatchByIdAsync(id);
        }

        public async Task<Match> CreateMatchAsync(Guid roomId)
        {
            var room = await GetRoomByIdAsync(roomId);
            if (room.Match != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "Match already in progress");
            }

            var game = await _applicationDbContext.Games!.Where(g => g.Id == room.GameId).FirstOrDefaultAsync();
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var match = new Match()
            {
                Room = room,
                GameId = room.GameId,
                Region = room.Region
            };

            await _applicationDbContext.AddAsync(match);
            await _applicationDbContext.SaveChangesAsync();

            Random random = new ();
            var roomUsers = room.RoomUsers!.Select(ru => ru.ApplicationUserId).ToList();

            for (int i = 1; i <= game.TeamCount; ++i)
            {
                var matchTeam = new MatchTeam()
                {
                    MatchId = match.Id,
                    TeamIndex = i,
                };

                await _applicationDbContext.AddAsync(matchTeam);
                await _applicationDbContext.SaveChangesAsync();

                var captainIndex = random.Next(roomUsers.Count - 1);
                var captainId = roomUsers.ElementAt(captainIndex);
                roomUsers.RemoveAt(captainIndex);

                var matchTeamCaptain = new MatchTeamUser()
                {
                    IsCaptain = true,
                    MatchTeamId = matchTeam.Id,
                    ApplicationUserId = captainId
                };

                if (i == 1)
                {
                    match.LobbyLeaderId = captainId;
                }

                _applicationDbContext.Update(match);
                await _applicationDbContext.AddAsync(matchTeamCaptain);
                await _applicationDbContext.SaveChangesAsync();
            }

            await _hubService.OnMatchChangedAsync(match, room);
            return match;
        }

        public async Task<Match> PickPlayerAsync(Guid userId, Guid id, Guid pickedUserId)
        {
            var match = await GetMatchByIdAsync(id);
            var room = await GetRoomByIdAsync(match!.RoomId!.Value);

            _delayedTasksService.CancelTask(match.RoomId!.Value);

            if (!room!.RoomUsers!.Any(ru => ru.ApplicationUserId == pickedUserId))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (match.MatchTeams!.Any(mt => mt.MatchTeamUsers!.Any(mtu => mtu.ApplicationUserId == pickedUserId)))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var teamToSelect = match.MatchTeams!.OrderBy(mt => mt.MatchTeamUsers!.Count).ThenBy(mt => mt.TeamIndex).First();
            if (!teamToSelect.MatchTeamUsers!.Any(tmu => tmu.ApplicationUserId == userId && tmu.IsCaptain))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var teamMember = new MatchTeamUser()
            {
                ApplicationUserId = pickedUserId,
                MatchTeamId = teamToSelect.Id
            };

            await _applicationDbContext.AddAsync(teamMember);
            await _applicationDbContext.SaveChangesAsync();

            await _hubService.OnMatchChangedAsync(match, room);

            return match;
        }

        public async Task<Match> SetLobbyIdAsync(Guid userId, Guid id, string lobbyId)
        {
            var match = await GetMatchByIdAsync(id);
            var matchTeamUser = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!)
                .FirstOrDefault(mtu => mtu.ApplicationUserId == userId);

            if (matchTeamUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            if (match.LobbyId != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            match.LobbyId = lobbyId;
            _applicationDbContext.Update(match);
            await _applicationDbContext.SaveChangesAsync();

            matchTeamUser.HasJoined = true;
            _applicationDbContext.Update(matchTeamUser);
            await _applicationDbContext.SaveChangesAsync();

            await _hubService.OnJoinLobbyAsync(match);
            return match;
        }

        public async Task<Match> VoteMapAsync(Guid userId, Guid id, string voteMap)
        {
            var match = await GetMatchByIdAsync(id);
            var matchTeamUser = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!)
                .FirstOrDefault(mtu => mtu.ApplicationUserId == userId);

            if (matchTeamUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            matchTeamUser.MapVote = voteMap;
            _applicationDbContext.Update(matchTeamUser);
            await _applicationDbContext.SaveChangesAsync();

            if (match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).Select(mtu => mtu.MapVote).All(m => !string.IsNullOrEmpty(m)))
            {
                _delayedTasksService.CancelTask(match.RoomId!.Value);
                await _hubService.OnCreateLobbyAsync(match);
            }

            return match;
        }

        public async Task<Match> HasJoinedAsync(Guid userId, Guid id)
        {
            var match = await GetMatchByIdAsync(id);
            var matchTeamUser = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!)
                .FirstOrDefault(mtu => mtu.ApplicationUserId == userId);

            if (matchTeamUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            matchTeamUser.HasJoined = true;
            _applicationDbContext.Update(matchTeamUser);
            await _applicationDbContext.SaveChangesAsync();

            if (match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).All(mtu => mtu.HasJoined))
            {
                await _hubService.OnSetupLobbyAsync(match);
            }

            return match;
        }

        public async Task<Match> MatchEndedAsync(Guid userId, Guid id, MatchEndedRequest request)
        {
            var match = await GetMatchByIdAsync(id);

            if (match.RoomId == null)
            {
                return match;
            }

            var matchTeamUser = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!)
                .FirstOrDefault(mtu => mtu.ApplicationUserId == userId);

            if (matchTeamUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var roomId = match.RoomId.Value;

            match.RoomId = null;
            _applicationDbContext.Update(match);

            var room = await GetRoomByIdAsync(roomId);
            foreach (var roomUser in room.RoomUsers!)
            {
                roomUser.IsReady = false;
                _applicationDbContext.Update(roomUser);
            }

            await _applicationDbContext.SaveChangesAsync();

            await _hubService.OnMatchEndedAsync(roomId);
            await _hubService.OnRoomChangedAsync(room);

            return match;
        }

        private async Task<Match> GetMatchByIdAsync(Guid id)
        {
            var match = await _applicationDbContext.Matches!
                    .Where(m => m.Id == id)
                    .Include(m => m.Game!)
                    .ThenInclude(g => g.Maps!)
                    .Include(m => m.MatchTeams!)
                    .ThenInclude(mt => mt.MatchTeamUsers!)
                    .ThenInclude(mtu => mtu.ApplicationUser!)
                    .ThenInclude(au => au!.GameUsers!)
                    .FirstOrDefaultAsync();

            if (match == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Match not found");
            }

            return match;
        }

        private async Task<Room> GetRoomByIdAsync(Guid roomId)
        {
            var room = await _applicationDbContext.Rooms!
                .Include(r => r.Game!)
                .ThenInclude(g => g.Maps!)
                .Include(r => r.Match!)
                .Include(r => r.RoomUsers!)
                .ThenInclude(ru => ru.ApplicationUser!)
                .ThenInclude(au => au!.GameUsers!)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Room not found");
            }

            return room;
        }
    }
}
