using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class CreateGameAdminRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string IconUrl { get; set; } = string.Empty;

        [Required]
        public int TeamCount { get; set; }

        [Required]
        public int PlayerCount { get; set; }
    }
}
