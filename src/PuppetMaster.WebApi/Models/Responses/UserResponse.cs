namespace PuppetMaster.WebApi.Models.Responses
{
    public class UserResponse
    {
        public Guid Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? AvatarUrl { get; set; }

        public List<GameUserResponse>? GameUsers { get; set; }

        public Guid? RoomUserId { get; set; }

        public Guid? RoomId { get; set; }

        public Guid? MatchId { get; set; }
    }
}
