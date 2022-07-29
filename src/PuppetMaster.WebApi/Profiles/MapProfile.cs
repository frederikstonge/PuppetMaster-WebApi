using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Map, MapResponse>();
        }
    }
}
