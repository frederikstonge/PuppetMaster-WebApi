namespace PuppetMaster.WebApi.Models.Database
{
    public class Game : EntityBase
    {
        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        public int TeamCount { get; set; }

        public int PlayerCount { get; set; }

        public ICollection<GameUser>? GameUsers { get; set; }

        public ICollection<Room>? Rooms { get; set; }

        public ICollection<Match>? Matches { get; set; }

        public ICollection<Map>? Maps { get; set; }
    }
}
