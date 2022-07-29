using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PuppetMaster.WebApi.Attributes;
using PuppetMaster.WebApi.Models;
using PuppetMaster.WebApi.Models.Requests;
using PuppetMaster.WebApi.Models.Responses;
using PuppetMaster.WebApi.Services;

namespace PuppetMaster.WebApi.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task Register([FromBody] RegisterRequest model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.RegisterAsync(model);
            }
        }

        [HttpPut("password/reset")]
        public async Task ResetPassword([FromBody] ResetPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.ResetPasswordAsync(model);
            }
        }

        [HttpPost("password/reset")]
        public async Task ResetPasswordRequest([FromBody, Required, EmailAddress] string email)
        {
            if (ModelState.IsValid)
            {
                await _accountService.ResetPasswordRequestAsync(email);
            }
        }

        [HttpPut("password")]
        [CustomAuthorize]
        public async Task ChangePassword([FromBody] ChangePasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.ChangePasswordAsync(model, User);
            }
        }

        [HttpPut("")]
        [CustomAuthorize]
        public async Task UpdateUser([FromBody] UpdateUserRequest model)
        {
            if (ModelState.IsValid)
            {
                await _accountService.UpdateUserAsync(model, User);
            }
        }

        [HttpGet("")]
        [CustomAuthorize]
        public async Task<UserResponse> GetUser()
        {
            var user = await _accountService.GetUserAsync(User);
            return _mapper.Map<UserResponse>(user);
        }

        [HttpPut("{id}/role")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task AddUserToRole(Guid id, string role)
        {
            var applicationUser = await _accountService.GetUserByIdAsync(id);
            if (applicationUser == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            await _accountService.AddToRoleAsync(applicationUser, role);
        }

        [HttpDelete("{id}/role")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task RemoveUserFromRole(Guid id, string role)
        {
            var applicationUser = await _accountService.GetUserByIdAsync(id);
            if (applicationUser == null)
            {
                throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
            }

            await _accountService.RemoveFromRoleAsync(applicationUser, role);
        }

        [HttpPut("{id}/password")]
        [CustomAuthorize(Roles = Role.Administrator)]
        public async Task SetPassword(Guid id, [FromBody] SetPasswordRequest model)
        {
            if (ModelState.IsValid)
            {
                var applicationUser = await _accountService.GetUserByIdAsync(id);
                if (applicationUser == null)
                {
                    throw new HttpResponseException(System.Net.HttpStatusCode.NotFound);
                }

                await _accountService.SetPasswordAsync(model, applicationUser);
            }
        }
    }
}
