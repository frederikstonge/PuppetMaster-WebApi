namespace PuppetMaster.WebApi.Models.Database
{
    public class MatchTeamUser : EntityBase
    {
        public Guid ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public Guid MatchTeamId { get; set; }

        public MatchTeam? MatchTeam { get; set; }

        public bool IsCaptain { get; set; }

        public string? MapVote { get; set; }

        public bool HasJoined { get; set; }
    }
}