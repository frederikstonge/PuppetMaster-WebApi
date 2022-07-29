namespace PuppetMaster.WebApi.Models.Database
{
    public class Room : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string? Password { get; set; }

        public string Region { get; set; } = string.Empty;

        public ICollection<RoomUser>? RoomUsers { get; set; }

        public Guid GameId { get; set; }

        public Game? Game { get; set; }

        public Match? Match { get; set; }
    }
}
