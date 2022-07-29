using AutoMapper;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Profiles
{
    public class MatchTeamProfile : Profile
    {
        public MatchTeamProfile()
        {
            CreateMap<MatchTeam, MatchTeamResponse>();
        }
    }
}
