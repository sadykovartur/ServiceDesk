using ServiceDesk.API.DTOs.Categories;

namespace ServiceDesk.API.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryResponse>> GetAllAsync(bool includeInactive, bool isStudent);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<CategoryResponse> SetActiveAsync(int id, SetActiveCategoryRequest request);
}
