namespace PuppetMaster.WebApi.Models.Database
{
    public class MatchTeam : EntityBase
    {
        public int TeamIndex { get; set; }

        public Guid MatchId { get; set; }

        public Match? Match { get; set; }

        public ICollection<MatchTeamUser>? MatchTeamUsers { get; set; }
    }
}
