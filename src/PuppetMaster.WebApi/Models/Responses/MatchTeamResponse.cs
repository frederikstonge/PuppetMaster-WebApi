namespace PuppetMaster.WebApi.Models.Responses
{
    public class MatchTeamResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public int TeamIndex { get; set; }

        public Guid MatchId { get; set; }

        public List<MatchTeamUserResponse>? MatchTeamUsers { get; set; }
    }
}
