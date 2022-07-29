namespace PuppetMaster.WebApi.Models.Database
{
    public class GameUser : EntityBase
    {
        public string UniqueGameId { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public Guid ApplicationUserId { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public Guid GameId { get; set; }

        public Game? Game { get; set; }
    }
}
