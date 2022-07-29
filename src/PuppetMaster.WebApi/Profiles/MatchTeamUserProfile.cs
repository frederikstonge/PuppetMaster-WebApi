using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class MatchTeamUserProfile : Profile
    {
        public MatchTeamUserProfile()
        {
            CreateMap<MatchTeamUser, MatchTeamUserResponse>();
        }
    }
}
