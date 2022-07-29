using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class CreateGameUserAdminRequest : CreateGameUserRequest
    {
        [Required]
        public Guid ApplicationUserId { get; set; }
    }
}
