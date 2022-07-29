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
    public class MapsController : ControllerBase
    {
        private readonly IMapsService _mapsService;
        private readonly IMapper _mapper;

        public MapsController(IMapsService mapService, IMapper mapper)
        {
            _mapsService = mapService;
            _mapper = mapper;
        }

        [HttpPost("admin")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task<MapResponse> CreateGameMapAdmin(CreateMapRequest request)
        {
            var map = await _mapsService.CreateGameMapAdminAsync(request);
            return _mapper.Map<MapResponse>(map);
        }

        [HttpPut("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task<MapResponse> UpdateGameMapAdmin(Guid id, UpdateMapRequest request)
        {
            var map = await _mapsService.UpdateGameMapAdminAsync(id, request);
            return _mapper.Map<MapResponse>(map);
        }

        [HttpDelete("admin/{id}")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public Task DeleteGameMapAdmin(Guid id)
        {
            return _mapsService.DeleteGameMapAdminAsync(id);
        }
    }
}
