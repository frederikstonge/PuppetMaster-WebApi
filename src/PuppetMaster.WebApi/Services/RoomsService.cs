using System.Net;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Helpers;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Repositories;

namespace PuppetMaster.WebApi.Services
{
    public class RoomsService : IRoomsService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHubService _hubService;

        public RoomsService(ApplicationDbContext applicationDbContext, IHubService hubService)
        {
            _applicationDbContext = applicationDbContext;
            _hubService = hubService;
        }

        public Task<List<Room>> GetRoomsAsync(Guid? gameId, string? region)
        {
            var rooms = _applicationDbContext.Rooms!.AsQueryable();
            if (gameId != null)
            {
                rooms = rooms.Where(r => r.GameId == gameId);
            }

            if (!string.IsNullOrEmpty(region))
            {
                rooms = rooms.Where(r => r.Region == region);
            }

            return rooms.Include(r => r.Game)
                .Include(r => r.RoomUsers!)
                .ThenInclude(ru => ru.ApplicationUser!)
                .ThenInclude(u => u.GameUsers!)
                .ToListAsync();
        }

        public Task<Room> GetRoomAsync(Guid id)
        {
            return GetRoomByIdAsync(id);
        }

        public async Task<Room> CreateRoomAsync(Guid userId, CreateRoomRequest request)
        {
            var game = await _applicationDbContext.Games!.Where(g => g.Id == request.GameId).FirstOrDefaultAsync();
            if (game == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var gameUser = await GetGameUserAsync(userId, request.GameId);
            if (gameUser.ApplicationUser!.RoomUser != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "User already in a room");
            }

            if (_applicationDbContext.Rooms!.Any(l => l.Name == request.Name && l.GameId == request.GameId))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, $"Room with name {request.Name} already exists");
            }

            var room = new Room()
            {
                Name = request.Name,
                Password = Base64Helper.Encode(request.Password),
                GameId = request.GameId,
                Region = gameUser.Region
            };

            await _applicationDbContext.AddAsync(room);
            await _applicationDbContext.SaveChangesAsync();

            room = await GetRoomByIdAsync(room.Id);

            var roomUser = new RoomUser()
            {
                RoomId = room.Id,
                ApplicationUserId = userId,
            };

            await _applicationDbContext.AddAsync(roomUser);
            await _applicationDbContext.SaveChangesAsync();

            return room;
        }

        public async Task<Room> JoinRoomAsync(Guid userId, Guid id, string? password)
        {
            var room = await GetRoomByIdAsync(id);
            if (room.Password != Base64Helper.Encode(password))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "Invalid password");
            }

            if (room.RoomUsers!.Count >= room.Game!.PlayerCount)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden, "Room is full");
            }

            if (room.Match != null)
            {
                var match = await GetMatchByIdAsync(room.Match.Id);
                if (match.MatchTeams!.SelectMany(mt => mt.MatchTeamUsers!).All(mtu => mtu.ApplicationUserId != userId))
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden, "Match in progress");
                }
            }

            var gameUser = await GetGameUserAsync(userId, room.GameId);
            if (gameUser.ApplicationUser!.RoomUser != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "User already in a room");
            }

            if (room.Region != gameUser.Region)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, $"Player is not from region {room.Region}");
            }

            var roomUser = new RoomUser()
            {
                RoomId = room.Id,
                ApplicationUserId = userId
            };

            await _applicationDbContext.AddAsync(roomUser);
            await _applicationDbContext.SaveChangesAsync();
            await _hubService.OnRoomChangedAsync(room);
            return room;
        }

        public async Task<Room?> LeaveRoomAsync(Guid userId, Guid id)
        {
            var room = await GetRoomByIdAsync(id);
            var gameUser = await GetGameUserAsync(userId, room.GameId);

            if (gameUser.ApplicationUser!.RoomUser == null || gameUser.ApplicationUser!.RoomUser!.RoomId != id)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "User not in this room");
            }

            var roomUser = room.RoomUsers!.First(ru => ru.Id == gameUser.ApplicationUser!.RoomUser!.Id);

            room.RoomUsers!.Remove(roomUser);
            await _applicationDbContext.SaveChangesAsync();

            if (!room.RoomUsers!.Any())
            {
                _applicationDbContext.Rooms!.Remove(room);
                await _applicationDbContext.SaveChangesAsync();
                return null;
            }

            await _hubService.OnRoomChangedAsync(room);
            return room;
        }

        public async Task<Room> ReadyRoomAsync(Guid userId, Guid id, bool isReady)
        {
            var room = await GetRoomByIdAsync(id);
            var gameUser = await GetGameUserAsync(userId, room.GameId);

            if (gameUser.ApplicationUser!.RoomUser == null || gameUser.ApplicationUser!.RoomUser!.RoomId != id)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "User not in this room");
            }

            if (room.Match != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict, "Match in progress");
            }

            var roomUser = room.RoomUsers!.First(ru => ru.Id == gameUser.ApplicationUser!.RoomUser!.Id);
            roomUser.IsReady = isReady;
            _applicationDbContext.Update(roomUser);
            await _applicationDbContext.SaveChangesAsync();
            await _hubService.OnRoomChangedAsync(room);
            return room;
        }

        private async Task<Room> GetRoomByIdAsync(Guid id)
        {
            var room = await _applicationDbContext.Rooms!
                    .Where(r => r.Id == id)
                    .Include(r => r.Game!)
                    .ThenInclude(g => g.Maps!)
                    .Include(r => r.Match!)
                    .Include(r => r.RoomUsers!)
                    .ThenInclude(ru => ru.ApplicationUser!)
                    .ThenInclude(au => au.GameUsers!)
                    .FirstOrDefaultAsync();

            if (room == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Room not found");
            }

            return room;
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

        private async Task<GameUser> GetGameUserAsync(Guid userId, Guid gameId)
        {
            var gameUser = await _applicationDbContext.GameUsers!
                .Where(gu => gu.ApplicationUserId == userId && gu.GameId == gameId)
                .Include(gu => gu.ApplicationUser!)
                .FirstOrDefaultAsync();

            if (gameUser == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, "Account not linked");
            }

            return gameUser;
        }
    }
}
