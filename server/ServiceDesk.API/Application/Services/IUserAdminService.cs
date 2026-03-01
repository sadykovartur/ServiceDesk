using ServiceDesk.API.DTOs.Users;

namespace ServiceDesk.API.Application.Services;

public interface IUserAdminService
{
    Task<IEnumerable<UserResponse>> GetAllAsync();
    Task<UserResponse> UpdateRoleAsync(string userId, UpdateUserRoleRequest request);
}
