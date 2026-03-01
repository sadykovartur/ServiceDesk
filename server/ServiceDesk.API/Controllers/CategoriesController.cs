using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceDesk.API.Application.Services;
using ServiceDesk.API.DTOs.Categories;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Get categories. Students always receive only active categories.
    /// Operators and Admins may pass includeInactive=true to see all.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(
        [FromQuery] bool includeInactive = false)
    {
        var isStudent = (User.FindFirstValue("role") ?? string.Empty) == "Student";
        return Ok(await _categoryService.GetAllAsync(includeInactive, isStudent));
    }

    /// <summary>Create a new category. Admin only.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Update category name. Admin only.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] UpdateCategoryRequest request)
    {
        return Ok(await _categoryService.UpdateAsync(id, request));
    }

    /// <summary>Toggle category active state. Admin only.</summary>
    [HttpPatch("{id:int}/active")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> SetActive(int id, [FromBody] SetActiveCategoryRequest request)
    {
        return Ok(await _categoryService.SetActiveAsync(id, request));
    }
}
