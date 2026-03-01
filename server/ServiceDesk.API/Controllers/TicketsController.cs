using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.API.Application.Services;
using ServiceDesk.API.DTOs;
using ServiceDesk.API.DTOs.Tickets;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
[Produces("application/json")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    /// <summary>Create a new ticket. Student only.</summary>
    [HttpPost]
    [Authorize(Roles = "Student")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TicketResponse>> Create([FromBody] CreateTicketRequest request)
    {
        var userId = User.FindFirstValue("sub")!;
        var role   = User.FindFirstValue("role") ?? string.Empty;
        var result = await _ticketService.CreateAsync(request, userId, role);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// List tickets with optional filters and pagination.
    /// Students always see only their own tickets; assignedToMe is ignored for Students.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<TicketResponse>>> GetAll(
        [FromQuery] string? status       = null,
        [FromQuery] string? priority     = null,
        [FromQuery] int?    categoryId   = null,
        [FromQuery] string? search       = null,
        [FromQuery] bool    assignedToMe = false,
        [FromQuery] int     page         = 1,
        [FromQuery] int     pageSize     = 20)
    {
        var userId = User.FindFirstValue("sub")!;
        var role   = User.FindFirstValue("role") ?? string.Empty;
        var query  = new TicketsQuery(status, priority, categoryId, search, assignedToMe, page, pageSize);
        return Ok(await _ticketService.GetAllAsync(query, userId, role));
    }

    /// <summary>
    /// Get a single ticket by id.
    /// Students can only access their own ticket — foreign ticket returns 404.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> GetById(int id)
    {
        var userId = User.FindFirstValue("sub")!;
        var role   = User.FindFirstValue("role") ?? string.Empty;
        return Ok(await _ticketService.GetByIdAsync(id, userId, role));
    }

    /// <summary>
    /// Assign ticket to the current operator.
    /// Promotes status from New → InProgress automatically.
    /// Anti-steal: 409 if already assigned to a different operator.
    /// </summary>
    [HttpPost("{id:int}/assign-to-me")]
    [Authorize(Roles = "Operator")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketResponse>> AssignToMe(int id)
    {
        var operatorId = User.FindFirstValue("sub")!;
        return Ok(await _ticketService.AssignToMeAsync(id, operatorId));
    }

    /// <summary>
    /// Change ticket status. Operator must be the assignee.
    /// Allowed transitions: InProgress ↔ WaitingForStudent, InProgress/WaitingForStudent → Resolved.
    /// New → InProgress is NOT allowed here; use assign-to-me instead.
    /// </summary>
    [HttpPost("{id:int}/status")]
    [Authorize(Roles = "Operator")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> ChangeStatus(int id, [FromBody] ChangeStatusRequest request)
    {
        var operatorId = User.FindFirstValue("sub")!;
        return Ok(await _ticketService.ChangeStatusAsync(id, operatorId, request.Status));
    }

    /// <summary>
    /// Reject a ticket with a mandatory reason.
    /// Anti-steal: 409 if assigned to a different operator.
    /// Auto-assigns if New with no assignee.
    /// </summary>
    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = "Operator")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketResponse>> Reject(int id, [FromBody] RejectRequest request)
    {
        var operatorId = User.FindFirstValue("sub")!;
        var reason     = request.Reason?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(reason))
            return BadRequest(new ProblemDetails
            {
                Status   = StatusCodes.Status400BadRequest,
                Title    = "Bad Request",
                Detail   = "Reject reason is required and must not be blank.",
                Instance = HttpContext.Request.Path
            });
        return Ok(await _ticketService.RejectAsync(id, operatorId, reason));
    }
}
