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
}
