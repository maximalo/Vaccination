using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vaccination.Application.Dtos;
using Vaccination.Application.Dtos.User;
using Vaccination.Application.Interfaces;
using Vaccination.Application.Shared;
using Vaccination.Application.Validators.User;
using Vaccination.Domain.Entities;

namespace Vaccination.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Consumes("application/json")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize(Roles = StaticUserRoles.ADMIN)]
        [Authorize(Roles = StaticUserRoles.WRITE)]
        [HttpPost]
        [Route("SetToAdmin")]
        public async Task<IActionResult> UpdateToRoleAdmin([FromBody] UpdateRoleRequest updateRoleRequest)
        {
            var adminResult = await userService.UpdateToAdminAsync(updateRoleRequest);

            if (adminResult.IsSucceed)
            {
                return Ok(adminResult);
            }

            return BadRequest(adminResult);
        }

        [Authorize(Roles = StaticUserRoles.USER)]
        [Authorize(Roles = StaticUserRoles.READ)]
        [HttpGet]
        [Route("GetUser")]
        public async Task<IActionResult> GetUser()
        {
            ApiResponse<UserDetailsResponse> response = new();

            var userIdClaim = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Invalid token");
            }

            // Create a request object to fetch user details
            var userDetailsRequest = new UserDetailsRequest { UserId = userIdClaim };

            GetUserValidator validator = new();
            await validator.ValidateAndThrowAsync(userDetailsRequest);

            var userDetails = await userService.GetUserDetails(userDetailsRequest);

            response.Data = userDetails;
            response.Message = "User details fetched successfully";

            return Ok(response);
        }

        [Authorize(Roles = StaticUserRoles.USER)]
        [Authorize(Roles = StaticUserRoles.WRITE)]
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            ApiResponse<UpdateUserResponse> response = new();

            var userIdClaim = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Invalid token");
            }

            UpdateUserValidator validator = new();
            await validator.ValidateAndThrowAsync(updateUserRequest);

            var updateUserResult = await userService.UpdateUserDetails(userIdClaim, updateUserRequest);

            response.Data = updateUserResult;
            response.Message = "User details updated successfully";

            return Ok(response);
        }

        [Authorize(Roles = StaticUserRoles.USER)]
        [Authorize(Roles = StaticUserRoles.DELETE)]
        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> DeleteAccount()
        {
            ApiResponse<DeleteUserResponse> response = new();

            var userIdClaim = User.FindFirst("userid")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Invalid token");
            }

            // Create a request object to fetch user details
            var deleteUserRequest = new DeleteUserRequest { UserId = userIdClaim };

            DeleteUserValidator validator = new();
            await validator.ValidateAndThrowAsync(deleteUserRequest);

            var deleteAccountResult = await userService.DeleteUserAsync(deleteUserRequest);

            response.Message = "User deleted successfully";

            return Ok(response);
        }
    }
}