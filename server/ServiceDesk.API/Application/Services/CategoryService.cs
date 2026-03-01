using Microsoft.EntityFrameworkCore;
using ServiceDesk.API.Application.Mapping;
using ServiceDesk.API.Domain;
using ServiceDesk.API.DTOs.Categories;
using ServiceDesk.API.Exceptions;
using ServiceDesk.API.Infrastructure.Data;

namespace ServiceDesk.API.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(AppDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IEnumerable<CategoryResponse>> GetAllAsync(bool includeInactive, bool isStudent)
    {
        IQueryable<Category> query = _db.Categories;

        if (isStudent || !includeInactive)
        {
            if (isStudent && includeInactive)
                _logger.LogWarning("Student attempted to use includeInactive=true — flag ignored");
            query = query.Where(c => c.IsActive);
        }

        var list = await query.OrderBy(c => c.Name).ToListAsync();
        return list.Select(c => c.ToResponse());
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Name must not be empty or whitespace.");

        var category = new Category { Name = name, IsActive = true };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return category.ToResponse();
    }

    public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request)
    {
        var category = await _db.Categories.FindAsync(id)
            ?? throw new NotFoundException($"Category {id} not found.");

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            throw new BusinessException("Name must not be empty or whitespace.");

        category.Name = name;
        await _db.SaveChangesAsync();

        return category.ToResponse();
    }

    public async Task<CategoryResponse> SetActiveAsync(int id, SetActiveCategoryRequest request)
    {
        var category = await _db.Categories.FindAsync(id)
            ?? throw new NotFoundException($"Category {id} not found.");

        category.IsActive = request.IsActive;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Category {CategoryId} active state changed to {IsActive}",
            category.Id, category.IsActive);

        return category.ToResponse();
    }
}
