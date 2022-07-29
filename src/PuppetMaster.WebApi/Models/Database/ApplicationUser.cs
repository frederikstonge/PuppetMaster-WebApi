using Microsoft.AspNetCore.Identity;

namespace PuppetMaster.WebApi.Models.Database
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public ICollection<GameUser>? GameUsers { get; set; }

        public RoomUser? RoomUser { get; set; }

        public ICollection<MatchTeamUser>? MatchTeamUsers { get; set; }
    }
}
