using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PuppetMaster.WebApi.Attributes;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Models.Responses;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchesService _matchesService;
        private readonly IMapper _mapper;

        public MatchesController(IMatchesService matchesService, IMapper mapper)
        {
            _matchesService = matchesService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        [CustomAuthorize]
        public async Task<MatchResponse> GetMatchAsync(Guid id)
        {
            var match = await _matchesService.GetMatchAsync(id);
            return _mapper.Map<MatchResponse>(match);
        }

        [HttpPost("{id}/join")]
        [CustomAuthorize]
        public async Task<MatchResponse> HasJoinedAsync(Guid id)
        {
            var userId = HttpContext.User.GetUserId();
            var match = await _matchesService.HasJoinedAsync(userId, id);
            return _mapper.Map<MatchResponse>(match);
        }

        [HttpPost("{id}/pick")]
        [CustomAuthorize]
        public async Task<MatchResponse> PickPlayerAsync(Guid id, PickPlayerRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var match = await _matchesService.PickPlayerAsync(userId, id, request.PickedUserId);
            return _mapper.Map<MatchResponse>(match);
        }

        [HttpPost("{id}/lobby-id")]
        [CustomAuthorize]
        public async Task<MatchResponse> SetLobbyIdAsync(Guid id, SetLobbyIdRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var match = await _matchesService.SetLobbyIdAsync(userId, id, request.LobbyId);
            return _mapper.Map<MatchResponse>(match);
        }

        [HttpPost("{id}/vote-map")]
        [CustomAuthorize]
        public async Task<MatchResponse> VoteMapAsync(Guid id, VoteMapRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var match = await _matchesService.VoteMapAsync(userId, id, request.VoteMap);
            return _mapper.Map<MatchResponse>(match);
        }

        [HttpPost("{id}/stats")]
        [CustomAuthorize]
        public async Task<MatchResponse> MatchEndedAsync(Guid id, MatchEndedRequest request)
        {
            var userId = HttpContext.User.GetUserId();
            var match = await _matchesService.MatchEndedAsync(userId, id, request);
            return _mapper.Map<MatchResponse>(match);
        }
    }
}