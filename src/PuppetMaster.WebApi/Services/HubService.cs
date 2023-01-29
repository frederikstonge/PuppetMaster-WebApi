using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using PuppetMaster.WebApi.Hubs;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Messages;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Services
{
    public class HubService : IHubService
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<RoomHub> _roomHubContext;
        private readonly IDelayedTasksService _delayedTasksService;

        public HubService(IMapper mapper, IHubContext<RoomHub> roomHubContext, IDelayedTasksService delayedTasksService)
        {
            _mapper = mapper;
            _roomHubContext = roomHubContext;
            _delayedTasksService = delayedTasksService;
        }

        public Task OnRoomChangedAsync(Room room)
        {
            var groupId = room.Id!.ToString();
            var roomResponse = _mapper.Map<RoomResponse>(room);
            return _roomHubContext.Clients.Group(groupId).SendAsync(SignalRMethods.RoomChanged, roomResponse);
        }

        public Task OnMatchChangedAsync(Match match)
        {
            // Cancel SchedulePlayerPick
            _delayedTasksService.CancelTask(match.RoomId!.Value);

            var groupId = match.RoomId!.Value.ToString();
            var matchResponse = _mapper.Map<MatchResponse>(match);

            var delay = TimeSpan.FromSeconds(15);

            var roomMatchResponse = new RoomMatchMessage()
            {
                Delay = delay,
                Match = matchResponse
            };

            var pickedPlayers = matchResponse.MatchTeams!
                .SelectMany(mt => mt.MatchTeamUsers!
                .Select(mtu => mtu.ApplicationUserId));

            roomMatchResponse.AvailablePlayers = matchResponse.Users!
                .ExceptBy(pickedPlayers, u => u!.Id)
                .ToList();

            if (roomMatchResponse.AvailablePlayers.Any())
            {
                var teamTurn = matchResponse.MatchTeams!
                    .OrderBy(mt => mt.MatchTeamUsers!.Count)
                    .ThenBy(mt => mt.TeamIndex)
                    .First();

                roomMatchResponse.CaptainToPickThisTurn = teamTurn.MatchTeamUsers!
                    .Single(mtu => mtu.IsCaptain)
                    .ApplicationUserId;
            }

            if (roomMatchResponse.AvailablePlayers.Any())
            {
                _delayedTasksService.SchedulePlayerPick(roomMatchResponse.CaptainToPickThisTurn!.Value, matchResponse.Id, matchResponse.RoomId!.Value, delay);
            }
            else
            {
                _delayedTasksService.ScheduleCreateLobby(matchResponse.Id, matchResponse.RoomId!.Value, delay);
            }

            return _roomHubContext.Clients.Group(groupId).SendAsync(SignalRMethods.MatchChanged, roomMatchResponse);
        }

        public Task OnCreateLobbyAsync(Match match)
        {
            // Cancel ScheduleCreateLobby
            _delayedTasksService.CancelTask(match.RoomId!.Value);

            if (!string.IsNullOrEmpty(match.LobbyId))
            {
                return Task.CompletedTask;
            }

            var message = new CreateLobbyMessage()
            {
                MatchId = match.Id
            };

            return _roomHubContext.Clients.User(match.LobbyLeaderId!.Value.ToString()).SendAsync(SignalRMethods.CreateLobby, message);
        }

        public Task OnSetupLobbyAsync(Match match)
        {
            // Cancel HasJoinedTimeout
            _delayedTasksService.CancelTask(match.RoomId!.Value);

            var maps = match.Game!.Maps!.ToList();

            var map = maps
                .OrderByDescending(m => match.MatchTeams!
                    .SelectMany(mt => mt.MatchTeamUsers!)
                    .Where(mtu => mtu.MapVote == m.Name)
                    .Count())
                .First();

            var message = new SetupLobbyMessage()
            {
                Map = map.Name,
                MatchId = match.Id
            };

            return _roomHubContext.Clients.User(match.LobbyLeaderId!.Value.ToString()).SendAsync(SignalRMethods.SetupLobby, message);
        }

        public async Task OnJoinLobbyAsync(Match match)
        {
            // Wait 30 seconds for others to join, otherwise cancel the match
            _delayedTasksService.HasJoinedTimeout(match.Id, match.RoomId!.Value, TimeSpan.FromSeconds(30));

            foreach (var matchTeam in match.MatchTeams!.OrderBy(mt => mt.TeamIndex))
            {
                var users = matchTeam.MatchTeamUsers!.Select(mtu => mtu.ApplicationUser!).Where(au => au.Id != match.LobbyLeaderId);
                foreach (var user in users)
                {
                    var message = new JoinLobbyMessage()
                    {
                        LobbyId = match.LobbyId!,
                        MatchId = match.Id,
                        TeamIndex = matchTeam.TeamIndex
                    };

                    await _roomHubContext.Clients.User(user.Id.ToString()).SendAsync(SignalRMethods.JoinLobby, message);
                }

                await Task.Delay(1000);
            }
        }

        public Task OnMatchEndedAsync(Guid roomId)
        {
            var groupId = roomId.ToString();
            var message = new MatchEndedMessage();

            return _roomHubContext.Clients.Group(groupId).SendAsync(SignalRMethods.MatchEnded, message);
        }
    }
}
