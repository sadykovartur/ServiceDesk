using ServiceDesk.API.Domain;
using ServiceDesk.API.DTOs.Users;

namespace ServiceDesk.API.Application.Mapping;

public static class UserMapping
{
    public static UserResponse ToResponse(this ApplicationUser u, string role) =>
        new(u.Id, u.DisplayName, u.Email ?? string.Empty, role);
}
