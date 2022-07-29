using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class CreateRoomRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid GameId { get; set; }

        public string? Password { get; set; }
    }
}
