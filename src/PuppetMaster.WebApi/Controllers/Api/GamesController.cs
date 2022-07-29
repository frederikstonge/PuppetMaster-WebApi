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
    public class GamesController : ControllerBase
    {
        private readonly IGamesService _gamesService;
        private readonly IMapper _mapper;

        public GamesController(IGamesService gameService, IMapper mapper)
        {
            _gamesService = gameService;
            _mapper = mapper;
        }

        [HttpGet("")]
        [CustomAuthorize]
        public async Task<List<GameResponse>> GetGames()
        {
            var games = await _gamesService.GetGamesAsync();
            return games.Select(g => _mapper.Map<GameResponse>(g)).ToList();
        }

        [HttpGet("{id}")]
        [CustomAuthorize]
        public async Task<GameResponse> GetGame(Guid id)
        {
            var game = await _gamesService.GetGameAsync(id);
            return _mapper.Map<GameResponse>(game);
        }

        [HttpPost("admin")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task<GameResponse> CreateGameAdmin(CreateGameAdminRequest request)
        {
            var game = await _gamesService.CreateGameAdminAsync(request);
            return _mapper.Map<GameResponse>(game);
        }

        [HttpPut("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task UpdateGameAdmin(Guid id, UpdateGameAdminRequest request)
        {
            return _gamesService.UpdateGameAdminAsync(id, request);
        }

        [HttpDelete("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task DeleteGameAdmin(Guid id)
        {
            return _gamesService.DeleteGameAdminAsync(id);
        }
    }
}
