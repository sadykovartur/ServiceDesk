using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.API.DTOs.Users;
using ServiceDesk.API.Models;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private static readonly HashSet<string> ValidRoles = ["Student", "Operator", "Admin"];

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<ApplicationUser> userManager, ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>List all users with their current role. Admin only.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = _userManager.Users.ToList();

        var result = new List<UserResponse>(users.Count);
        foreach (var user in users.OrderBy(u => u.Email))
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;
            result.Add(new UserResponse(user.Id, user.DisplayName, user.Email ?? string.Empty, role));
        }

        return Ok(result);
    }

    /// <summary>Change a user's role. Admin only. Role takes effect on next login.</summary>
    [HttpPut("{id}/role")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> UpdateRole(string id, [FromBody] UpdateUserRoleRequest request)
    {
        if (!ValidRoles.Contains(request.Role))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["role"] = [$"Role must be one of: {string.Join(", ", ValidRoles)}."] }
            });
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"User {id} not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        var oldRole = currentRoles.FirstOrDefault() ?? string.Empty;

        if (currentRoles.Count > 0)
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

        await _userManager.AddToRoleAsync(user, request.Role);

        _logger.LogInformation("Role changed for user {UserId}: {OldRole} → {NewRole}",
            user.Id, oldRole, request.Role);

        return Ok(new UserResponse(user.Id, user.DisplayName, user.Email ?? string.Empty, request.Role));
    }
}
