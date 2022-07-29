namespace PuppetMaster.WebApi.Models.Database
{
    public class Map : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public Guid GameId { get; set; }

        public Game? Game { get; set; }
    }
}
