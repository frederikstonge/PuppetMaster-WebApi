using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Microsoft.AspNetCore.Http
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(Claims.Subject)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            throw new HttpResponseException(System.Net.HttpStatusCode.InternalServerError);
        }

        public static Guid? GetUserIdOrDefault(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(Claims.Subject)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return default;
        }
    }
}
