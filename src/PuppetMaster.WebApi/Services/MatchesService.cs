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

        public MatchesService(ApplicationDbContext applicationDbContext, IHubService hubService)
        {
            _applicationDbContext = applicationDbContext;
            _hubService = hubService;
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
                Region = room.Region,
                Users = room.RoomUsers!.Select(ru => ru.ApplicationUser!).ToList()
            };

            await _applicationDbContext.AddAsync(match);
            await _applicationDbContext.SaveChangesAsync();

            Random random = new ();

            // Get ids of all users in the room
            var roomUsers = room.RoomUsers!.Select(ru => ru.ApplicationUserId).ToList();

            for (int i = 1; i <= game.TeamCount; ++i)
            {
                // Create team
                var matchTeam = new MatchTeam()
                {
                    MatchId = match.Id,
                    TeamIndex = i,
                };

                await _applicationDbContext.AddAsync(matchTeam);
                await _applicationDbContext.SaveChangesAsync();

                // Select random index for team captain
                var captainIndex = random.Next(roomUsers.Count - 1);
                var captainId = roomUsers.ElementAt(captainIndex);
                roomUsers.RemoveAt(captainIndex);

                var matchTeamCaptain = new MatchTeamUser()
                {
                    IsCaptain = true,
                    MatchTeamId = matchTeam.Id,
                    ApplicationUserId = captainId
                };

                await _applicationDbContext.AddAsync(matchTeamCaptain);
                await _applicationDbContext.SaveChangesAsync();

                // If first team, captain is also lobby leader
                if (i == 1)
                {
                    match.LobbyLeaderId = captainId;
                    _applicationDbContext.Update(match);
                    await _applicationDbContext.SaveChangesAsync();
                }
            }

            // Notify match has changed
            await _hubService.OnRoomChangedAsync(room);
            await _hubService.OnMatchChangedAsync(match);
            return match;
        }

        public async Task<Match> PickPlayerAsync(Guid userId, Guid id, Guid pickedUserId)
        {
            var match = await GetMatchByIdAsync(id);

            // Picked user is in the match
            if (!match!.Users!.Any(au => au.Id == pickedUserId))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            // Picked user is not already picked
            if (match.MatchTeams!.Any(mt => mt.MatchTeamUsers!.Any(mtu => mtu.ApplicationUserId == pickedUserId)))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            // Team turn to pick
            var teamToSelect = match.MatchTeams!
                .OrderBy(mt => mt.MatchTeamUsers!.Count)
                .ThenBy(mt => mt.TeamIndex)
                .First();

            // User is from team and is captain
            if (!teamToSelect.MatchTeamUsers!.Any(tmu => tmu.ApplicationUserId == userId && tmu.IsCaptain))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var teamMember = new MatchTeamUser()
            {
                ApplicationUserId = pickedUserId,
                MatchTeamId = teamToSelect.Id,
            };

            await _applicationDbContext.AddAsync(teamMember);
            await _applicationDbContext.SaveChangesAsync();

            // Notify match has changed
            await _hubService.OnMatchChangedAsync(match);

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

            // Notify other users to join
            await _hubService.OnJoinLobbyAsync(match);

            // Set yourself as joined
            await HasJoinedAsync(userId, id);

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

            // All players have voted, create lobby.
            if (match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).Select(mtu => mtu.MapVote).All(m => !string.IsNullOrEmpty(m)))
            {
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

            var matchTeamUser = match.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!)
                .FirstOrDefault(mtu => mtu.ApplicationUserId == userId);

            if (matchTeamUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            // Use request to save score and stats
            return await EndMatchAsync(match);
        }

        public async Task<Match> MatchAbandonnedAsync(Guid id)
        {
            var match = await GetMatchByIdAsync(id);
            return await EndMatchAsync(match);
        }

        private async Task<Match> GetMatchByIdAsync(Guid id)
        {
            var match = await _applicationDbContext.Matches!
                    .Where(m => m.Id == id)
                    .Include(m => m.Users)
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

        private async Task<Match> EndMatchAsync(Match match)
        {
            if (match.RoomId == null)
            {
                return match;
            }

            var roomId = match.RoomId.Value;

            match.Users!.Clear();
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
    }
}
