using ServiceDesk.Application.DTOs;

namespace ServiceDesk.Application.Interfaces.Services;

public interface ITicketService
{
    Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TicketDto?> GetByIdAsync(int ticketId, CancellationToken cancellationToken = default);
    Task<TicketDto> CreateAsync(CreateTicketRequestDto request, CancellationToken cancellationToken = default);
}
