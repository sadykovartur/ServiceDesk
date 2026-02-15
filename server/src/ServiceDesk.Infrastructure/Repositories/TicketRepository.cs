using Microsoft.EntityFrameworkCore;
using ServiceDesk.Application.Interfaces.Repositories;
using ServiceDesk.Domain.Entities;
using ServiceDesk.Infrastructure.Persistence;

namespace ServiceDesk.Infrastructure.Repositories;

public class TicketRepository(ApplicationDbContext dbContext) : ITicketRepository
{
    public async Task<IReadOnlyList<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Category)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Ticket?> GetByIdAsync(int ticketId, CancellationToken cancellationToken = default)
    {
        return dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == ticketId, cancellationToken);
    }

    public async Task<Ticket> CreateAsync(Ticket ticket, CancellationToken cancellationToken = default)
    {
        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ticket;
    }
}
