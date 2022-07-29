using System.Security.Claims;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public interface IAccountService
    {
        Task ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal user);

        Task UpdateUserAsync(UpdateUserRequest model, ClaimsPrincipal user);

        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user);

        Task RegisterAsync(RegisterRequest model);

        Task ResetPasswordAsync(ResetPasswordRequest model);

        Task ResetPasswordRequestAsync(string email);

        Task<ApplicationUser> GetUserByIdAsync(Guid id);

        Task AddToRoleAsync(ApplicationUser applicationUser, string role);

        Task RemoveFromRoleAsync(ApplicationUser applicationUser, string role);

        Task SetPasswordAsync(SetPasswordRequest model, ApplicationUser applicationUser);
    }
}