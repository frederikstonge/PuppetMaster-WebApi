using PuppetMaster.WebApi.Models.Responses;

namespace PuppetMaster.WebApi.Models.Messages
{
    public class RoomMatchMessage
    {
        public MatchResponse? Match { get; set; }

        public Guid? CaptainToPickThisTurn { get; set; }

        public List<UserResponse> AvailablePlayers { get; set; } = new List<UserResponse>();

        public TimeSpan? Delay { get; set; }
    }
}
