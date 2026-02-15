using Microsoft.AspNetCore.Mvc;
using ServiceDesk.Application.DTOs;
using ServiceDesk.Application.Interfaces.Services;

namespace ServiceDesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var tickets = await ticketService.GetAllAsync(cancellationToken);
        return Ok(tickets);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var ticket = await ticketService.GetByIdAsync(id, cancellationToken);
        if (ticket is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Ticket not found",
                Detail = $"Ticket with id {id} does not exist.",
                Status = StatusCodes.Status404NotFound
            });
        }

        return Ok(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var createdTicket = await ticketService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdTicket.Id }, createdTicket);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Category not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            });
        }
    }
}
