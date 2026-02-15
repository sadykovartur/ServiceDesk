using ServiceDesk.Domain.Entities;

namespace ServiceDesk.Application.Interfaces.Repositories;

public interface ITicketRepository
{
    Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Ticket?> GetByIdAsync(int ticketId, CancellationToken cancellationToken = default);
    Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken = default);
}
