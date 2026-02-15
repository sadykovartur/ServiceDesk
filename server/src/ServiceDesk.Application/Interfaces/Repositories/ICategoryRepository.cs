using ServiceDesk.Domain.Entities;

namespace ServiceDesk.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int categoryId, CancellationToken cancellationToken = default);
}
