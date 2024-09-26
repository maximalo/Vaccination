using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Vaccination.Application.Dtos.User;
using Vaccination.Application.Interfaces;
using Vaccination.Application.Shared;
using Vaccination.Domain.Entities;
using Vaccination.Domain.Interfaces;

namespace Vaccination.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork unitOfWork;

        public UserService(UserManager<User> userManager, IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _userManager = userManager;
            this.unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<UpdateRoleResponse> UpdateToAdminAsync(UpdateRoleRequest updateRoleRequest)
        {
            User? user = await _userManager.FindByEmailAsync(updateRoleRequest.Email);

            if (user is null)
            {
                return new UpdateRoleResponse()
                {
                    IsSucceed = false,
                    Message = "Invalid UserName"
                };
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return new UpdateRoleResponse()
            {
                IsSucceed = true,
                Message = "User is now Admin"
            };
        }

        public async Task<bool> DeleteUserAsync(DeleteUserRequest deleteUserRequest)
        {
            User? user = await _userManager.FindByIdAsync(deleteUserRequest.UserId);

            if (user is null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Add a suffix to the username and normalized username to avoid conflicts
            user.NormalizedUserName = $"{user.NormalizedUserName}_DELETED_{DateTime.Now:yyyyMMdd_HHmmss}";
            user.UserName = $"{user.UserName}_DELETED_{DateTime.Now:yyyyMMdd_HHmmss}";

            IdentityResult deleteResult = await _userManager.DeleteAsync(user);

            if (!deleteResult.Succeeded)
            {
                string errorString = "User Deletion Failed because : ";
                foreach (IdentityError error in deleteResult.Errors)
                {
                    errorString += " # " + error.Description;
                }

                throw new InvalidOperationException(errorString);
            }

            return true;
        }

        public async Task<UserDetailsResponse> GetUserDetails(UserDetailsRequest userDetailsRequest)
        {
            User? user = await _userManager.FindByIdAsync(userDetailsRequest.UserId);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return _mapper.Map<UserDetailsResponse>(user);
        }

        public async Task<UpdateUserResponse> UpdateUserDetails(string userId, UpdateUserRequest updateUserRequest)
        {
            User existingUser = await _userManager.FindByIdAsync(userId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            // Store the values of CreatedBy and CreatedOnUtc
            var createdBy = existingUser.CreatedBy;
            var createdOnUtc = existingUser.CreatedOnUtc;

            _mapper.Map(updateUserRequest, existingUser);

            // Restore the values of CreatedBy and CreatedOnUtc
            existingUser.CreatedBy = createdBy;
            existingUser.CreatedOnUtc = createdOnUtc;

            IdentityResult updateResult = await _userManager.UpdateAsync(existingUser);
            await unitOfWork.SaveAsync();

            return _mapper.Map<UpdateUserResponse>(existingUser);
        }
    }
}