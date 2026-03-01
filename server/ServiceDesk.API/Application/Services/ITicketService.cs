using ServiceDesk.API.DTOs;
using ServiceDesk.API.DTOs.Tickets;

namespace ServiceDesk.API.Application.Services;

public interface ITicketService
{
    Task<PagedResponse<TicketResponse>> GetAllAsync(TicketsQuery query, string userId, string role);
    Task<TicketResponse> GetByIdAsync(int id, string userId, string role);
    Task<TicketResponse> CreateAsync(CreateTicketRequest request, string userId, string role);
}
