using System.ComponentModel.DataAnnotations;

namespace PuppetMaster.WebApi.Models.Requests
{
    public class SetPasswordRequest
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
