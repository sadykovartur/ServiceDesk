using Microsoft.EntityFrameworkCore;
using ServiceDesk.Application.Interfaces.Repositories;
using ServiceDesk.Domain.Entities;
using ServiceDesk.Infrastructure.Persistence;

namespace ServiceDesk.Infrastructure.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        return dbContext.Categories.AnyAsync(x => x.Id == categoryId, cancellationToken);
    }
}
