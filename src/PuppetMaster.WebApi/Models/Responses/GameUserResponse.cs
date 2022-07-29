namespace PuppetMaster.WebApi.Models.Responses
{
    public class GameUserResponse
    {
        public Guid Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public Guid? ModifiedBy { get; set; }

        public string UniqueGameId { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public Guid ApplicationUserId { get; set; }

        public Guid GameId { get; set; }

        public GameResponse? Game { get; set; }
    }
}
