﻿namespace PuppetMaster.WebApi.Models.Messages
{
    public class JoinLobbyMessage
    {
        public string LobbyId { get; set; } = string.Empty;

        public Guid MatchId { get; set; }

        public int TeamIndex { get; set; }
    }
}
