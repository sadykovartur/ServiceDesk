using System.ComponentModel.DataAnnotations;

namespace ServiceDesk.API.DTOs.Tickets;

public record CreateTicketRequest(
    [Required] string Title,
    [Required] string Description,
    [Required] string Priority,
    int CategoryId
);
