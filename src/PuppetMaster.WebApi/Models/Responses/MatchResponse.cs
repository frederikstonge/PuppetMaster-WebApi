namespace PuppetMaster.WebApi.Models.Responses
{
    public class MatchResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public string Region { get; set; } = string.Empty;

        public List<MatchTeamResponse>? MatchTeams { get; set; }

        public Guid GameId { get; set; }

        public GameResponse? Game { get; set; }

        public Guid? RoomId { get; set; }

        public Guid? LobbyLeaderId { get; set; }

        public string? LobbyId { get; set; }
    }
}
