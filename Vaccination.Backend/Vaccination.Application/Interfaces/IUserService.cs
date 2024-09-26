using Vaccination.Application.Dtos.User;

namespace Vaccination.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDetailsResponse> GetUserDetails(UserDetailsRequest userDetailsRequest);

        Task<UpdateUserResponse> UpdateUserDetails(string userId, UpdateUserRequest updateUserRequest);

        Task<UpdateRoleResponse> UpdateToAdminAsync(UpdateRoleRequest updateRoleRequest);

        Task<bool> DeleteUserAsync(DeleteUserRequest deleteUserRequest);
    }
}