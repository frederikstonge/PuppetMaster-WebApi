using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class UpdateGameUserRequest
    {
        [Required]
        public string UniqueGameId { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;
    }
}
