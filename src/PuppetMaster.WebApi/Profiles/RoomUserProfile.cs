using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class RoomUserProfile : Profile
    {
        public RoomUserProfile()
        {
            CreateMap<RoomUser, RoomUserResponse>();
        }
    }
}
