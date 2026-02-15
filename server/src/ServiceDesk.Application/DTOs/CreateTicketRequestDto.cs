using System.ComponentModel.DataAnnotations;
using ServiceDesk.Domain.Enums;

namespace ServiceDesk.Application.DTOs;

public class CreateTicketRequestDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int CategoryId { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;

    [Required]
    [MaxLength(128)]
    public string AuthorId { get; set; } = string.Empty;
}
