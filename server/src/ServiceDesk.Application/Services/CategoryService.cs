using ServiceDesk.Application.DTOs;
using ServiceDesk.Application.Interfaces.Repositories;
using ServiceDesk.Application.Interfaces.Services;

namespace ServiceDesk.Application.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return categories
            .Select(x => new CategoryDto(x.Id, x.Name, x.IsActive))
            .ToList();
    }
}
