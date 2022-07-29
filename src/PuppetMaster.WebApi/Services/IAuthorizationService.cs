using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;

namespace PuppetMaster.WebApi.Services
{
    public interface IAuthorizationService
    {
        Task<ClaimsPrincipal> PasswordGrantAsync(OpenIddictRequest request);

        Task<ClaimsPrincipal> RefreshTokenGrantAsync(AuthenticateResult info);
    }
}
