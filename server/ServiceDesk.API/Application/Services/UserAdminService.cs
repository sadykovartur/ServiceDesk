using Microsoft.AspNetCore.Identity;
using ServiceDesk.API.Application.Mapping;
using ServiceDesk.API.Domain;
using ServiceDesk.API.DTOs.Users;
using ServiceDesk.API.Exceptions;

namespace ServiceDesk.API.Application.Services;

public class UserAdminService : IUserAdminService
{
    private static readonly HashSet<string> ValidRoles = ["Student", "Operator", "Admin"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UserAdminService> _logger;

    public UserAdminService(UserManager<ApplicationUser> userManager, ILogger<UserAdminService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = _userManager.Users.OrderBy(u => u.Email).ToList();

        var result = new List<UserResponse>(users.Count);
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;
            result.Add(user.ToResponse(role));
        }

        return result;
    }

    public async Task<UserResponse> UpdateRoleAsync(string userId, UpdateUserRoleRequest request)
    {
        if (!ValidRoles.Contains(request.Role))
            throw new BusinessException($"Role must be one of: {string.Join(", ", ValidRoles)}.");

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException($"User {userId} not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var oldRole = currentRoles.FirstOrDefault() ?? string.Empty;

        if (currentRoles.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, request.Role);

        _logger.LogInformation("Role changed for user {UserId}: {OldRole} → {NewRole}",
            user.Id, oldRole, request.Role);

        return user.ToResponse(request.Role);
    }
}
