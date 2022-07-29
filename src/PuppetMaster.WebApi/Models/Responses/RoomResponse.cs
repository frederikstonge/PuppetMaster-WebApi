namespace PuppetMaster.WebApi.Models.Responses
{
    public class RoomResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsPrivate { get; set; }

        public string Region { get; set; } = string.Empty;

        public List<RoomUserResponse>? RoomUsers { get; set; }

        public Guid GameId { get; set; }

        public GameResponse? Game { get; set; }

        public Guid? MatchId { get; set; }
    }
}
