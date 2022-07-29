using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using OpenIddict.Abstractions;
using PuppetMaster.WebApi.Models.Database;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace PuppetMaster.WebApi.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthorizationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<ClaimsPrincipal> PasswordGrantAsync(OpenIddictRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(request.Username);
                if (user == null)
                {
                    throw new HttpResponseException(HttpStatusCode.Forbidden);
                }
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            principal.SetScopes(
                new[]
                {
                    Scopes.OpenId,
                    Scopes.Email,
                    Scopes.Profile,
                    Scopes.OfflineAccess,
                    Scopes.Roles
                }.Intersect(request.GetScopes()));

            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return principal;
        }

        public async Task<ClaimsPrincipal> RefreshTokenGrantAsync(AuthenticateResult info)
        {
            var user = await _signInManager.ValidateSecurityStampAsync(info.Principal);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            if (!await _signInManager.CanSignInAsync(user))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(GetDestinations(claim, principal));
            }

            return principal;
        }

        private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
        {
            switch (claim.Type)
            {
                case Claims.Name:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Profile))
                    {
                        yield return Destinations.IdentityToken;
                    }

                    yield break;

                case Claims.Email:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Email))
                    {
                        yield return Destinations.IdentityToken;
                    }

                    yield break;

                case Claims.Role:
                    yield return Destinations.AccessToken;

                    if (principal.HasScope(Scopes.Roles))
                    {
                        yield return Destinations.IdentityToken;
                    }

                    yield break;

                case "AspNet.Identity.SecurityStamp": yield break;

                default:
                    yield return Destinations.AccessToken;
                    yield break;
            }
        }
    }
}
