using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PuppetMaster.WebApi.Attributes;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Models.Responses;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _roomsService;
        private readonly IMatchesService _matchesService;
        private readonly IMapper _mapper;

        public RoomsController(IRoomsService roomsService, IMatchesService matchesService, IMapper mapper)
        {
            _roomsService = roomsService;
            _matchesService = matchesService;
            _mapper = mapper;
        }

        [HttpGet("")]
        [CustomAuthorize]
        public async Task<List<RoomResponse>> GetRooms(Guid? gameId, string? region)
        {
            var rooms = await _roomsService.GetRoomsAsync(gameId, region);
            return rooms.Select(r => _mapper.Map<RoomResponse>(r)).ToList();
        }

        [HttpGet("{id}")]
        [CustomAuthorize]
        public async Task<RoomResponse> GetRoom(Guid id)
        {
            var room = await _roomsService.GetRoomAsync(id);
            return _mapper.Map<RoomResponse>(room);
        }

        [HttpPost("")]
        [CustomAuthorize]
        public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var room = await _roomsService.CreateRoomAsync(userId, request);
            return _mapper.Map<RoomResponse>(room);
        }

        [HttpPut("{id}")]
        [CustomAuthorize]
        public Task JoinRoom(Guid id, string? password)
        {
            var userId = HttpContext.User.GetUserId();
            return _roomsService.JoinRoomAsync(userId, id, password);
        }

        [HttpDelete("{id}")]
        [CustomAuthorize]
        public Task LeaveRoom(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            return _roomsService.LeaveRoomAsync(userId, id);
        }

        [HttpPut("{id}/ready")]
        [CustomAuthorize]
        public async Task ReadyRoom(Guid id, bool isReady)
        {
            var userId = HttpContext.User.GetUserId();
            var room = await _roomsService.ReadyRoomAsync(userId, id, isReady);
            if (room.RoomUsers!.Count == room.Game!.PlayerCount && room.RoomUsers!.All(ru => ru.IsReady))
            {
                await _matchesService.CreateMatchAsync(room.Id);
            }
        }

        [HttpDelete("{id}/admin/kick/{userId}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task KickUser(Guid id, Guid userId)
        {
            return _roomsService.LeaveRoomAsync(userId, id);
        }
    }
}
