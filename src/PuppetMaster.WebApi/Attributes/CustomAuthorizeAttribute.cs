using Microsoft.AspNetCore.Authorization;
using OpenIddict.Validation.AspNetCore;

namespace PuppetMaster.WebApi.Attributes
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        public CustomAuthorizeAttribute()
        {
            AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        }
    }
}
