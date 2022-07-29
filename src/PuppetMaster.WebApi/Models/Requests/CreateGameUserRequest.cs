using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class CreateGameUserRequest
    {
        [Required]
        public string UniqueGameId { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public Guid GameId { get; set; }
    }
}
