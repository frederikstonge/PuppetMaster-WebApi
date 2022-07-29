using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PuppetMaster.WebApi.Models.Database;
using PuppetMaster.WebApi.Models.Requests;

namespace PuppetMaster.WebApi.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task RegisterAsync(RegisterRequest model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            var email = await _userManager.FindByEmailAsync(model.Email);
            if (email != null)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AvatarUrl = model.AvatarUrl
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }
        }

        public async Task ResetPasswordAsync(ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            var result = await _userManager.ResetPasswordAsync(user, model.ResetPasswordToken, model.NewPassword);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }
        }

        public async Task ResetPasswordRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var result = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (string.IsNullOrEmpty(result))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        public async Task ChangePasswordAsync(ChangePasswordRequest model, ClaimsPrincipal user)
        {
            var applicationUser = await _userManager.GetUserAsync(user);

            var result = await _userManager.ChangePasswordAsync(applicationUser, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }
        }

        public async Task UpdateUserAsync(UpdateUserRequest model, ClaimsPrincipal user)
        {
            var applicationUser = await _userManager.GetUserAsync(user);

            applicationUser.FirstName = model.FirstName;
            applicationUser.LastName = model.LastName;

            var result = await _userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }
        }

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            return _userManager.Users
                .Include(u => u.GameUsers)
                .Include(u => u.RoomUser)
                .FirstAsync(u => u.Id == userId);
        }

        public Task<ApplicationUser> GetUserByIdAsync(Guid id)
        {
            return _userManager.FindByIdAsync(id.ToString());
        }

        public async Task AddToRoleAsync(ApplicationUser applicationUser, string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new ApplicationRole()
                {
                    Name = role,
                });
            }

            await _userManager.AddToRoleAsync(applicationUser, role);
        }

        public async Task RemoveFromRoleAsync(ApplicationUser applicationUser, string role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound, role);
            }

            await _userManager.RemoveFromRoleAsync(applicationUser, role);
        }

        public async Task SetPasswordAsync(SetPasswordRequest model, ApplicationUser applicationUser)
        {
            var result = await _userManager.RemovePasswordAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }

            result = await _userManager.AddPasswordAsync(applicationUser, model.Password);
            if (!result.Succeeded)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest, result.Errors);
            }
        }
    }
}
