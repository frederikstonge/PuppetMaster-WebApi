using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            CreateMap<Match, MatchResponse>()
                .ForMember(
                r => r.RoomId,
                opt => opt.MapFrom(r => r.Room!.Id));
        }
    }
}
