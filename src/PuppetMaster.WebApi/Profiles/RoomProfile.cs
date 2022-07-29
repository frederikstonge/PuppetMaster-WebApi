using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Room, RoomResponse>()
                .ForMember(
                r => r.IsPrivate, 
                opt => opt.MapFrom(r => !string.IsNullOrEmpty(r.Password)))
                .ForMember(
                r => r.MatchId,
                opt => opt.MapFrom(r => r.Match!.Id));
        }
    }
}
