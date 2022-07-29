namespace PuppetMaster.WebApi.Models.Messages
{
    public class SetupLobbyMessage
    {
        public string Map { get; set; } = string.Empty;

        public Guid MatchId { get; set; }
    }
}
