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
    public class GameUsersController : ControllerBase
    {
        private readonly IGameUsersService _gameUsersService;
        private readonly IMapper _mapper;

        public GameUsersController(IGameUsersService gameUsersService, IMapper mapper)
        {
            _gameUsersService = gameUsersService;
            _mapper = mapper;
        }

        [HttpGet("")]
        [CustomAuthorize]
        public async Task<List<GameUserResponse>> GetGameUsers()
        {
            var userId = HttpContext.User.GetUserId();
            var gameUsers = await _gameUsersService.GetGameUsersAsync(userId);
            return gameUsers.Select(g => _mapper.Map<GameUserResponse>(g)).ToList();
        }

        [HttpGet("{id}")]
        [CustomAuthorize]
        public async Task<GameUserResponse> GetGameUser(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            var gameUser = await _gameUsersService.GetGameUserAsync(userId, id);
            return _mapper.Map<GameUserResponse>(gameUser);
        }

        [HttpPost("")]
        [CustomAuthorize]
        public async Task<GameUserResponse> CreateGameUser(CreateGameUserRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var gameUser = await _gameUsersService.CreateGameUserAsync(userId, request);
            return _mapper.Map<GameUserResponse>(gameUser);
        }

        [HttpPut("{id}")]
        [CustomAuthorize]
        public Task UpdateGameUser(Guid id, UpdateGameUserRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            return _gameUsersService.UpdateGameUserAsync(userId, id, request);
        }

        [HttpPost("admin")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task<GameUserResponse> CreateGameUserAdmin(CreateGameUserAdminRequest request)
        {
            var gameUser = await _gameUsersService.CreateGameUserAdminAsync(request);
            return _mapper.Map<GameUserResponse>(gameUser);
        }

        [HttpPut("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task UpdateGameUserAdmin(Guid id, UpdateGameUserRequest request)
        {
            return _gameUsersService.UpdateGameUserAdminAsync(id, request);
        }

        [HttpDelete("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task DeleteGameUserAdmin(Guid id)
        {
            return _gameUsersService.DeleteGameUserAdminAsync(id);
        }
    }
}
