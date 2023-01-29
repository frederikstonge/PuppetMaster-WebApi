namespace PuppetMaster.WebApi.Models.Database
{
    public class Match : EntityBase
    {
        public string Region { get; set; } = string.Empty;

        public Guid GameId { get; set; }

        public Game? Game { get; set; }

        public ICollection<ApplicationUser>? Users { get; set; }

        public ICollection<MatchTeam>? MatchTeams { get; set; }

        public Room? Room { get; set; }

        public Guid? RoomId { get; set; }

        public Guid? LobbyLeaderId { get; set; }

        public string? LobbyId { get; set; }
    }
}
