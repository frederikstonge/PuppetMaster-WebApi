using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class GameUserProfile : Profile
    {
        public GameUserProfile()
        {
            CreateMap<GameUser, GameUserResponse>();
        }
    }
}
