using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceDesk.API.Data;
using ServiceDesk.API.DTOs.Categories;
using ServiceDesk.API.Models;

namespace ServiceDesk.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(AppDbContext db, ILogger<CategoriesController> logger)
    {
        _db = db;
        _logger = logger;
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
        var role = User.FindFirstValue("role") ?? string.Empty;
        var isStudentOrLower = role == "Student";

        IQueryable<Category> query = _db.Categories;

        if (isStudentOrLower || !includeInactive)
        {
            if (isStudentOrLower && includeInactive)
            {
                _logger.LogWarning("Student attempted to use includeInactive=true — flag ignored");
            }
            query = query.Where(c => c.IsActive);
        }

        var categories = await query
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name, c.IsActive))
            .ToListAsync();

        return Ok(categories);
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
        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["name"] = ["Name must not be empty or whitespace."] }
            });
        }

        var category = new Category { Name = name, IsActive = true };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        var response = new CategoryResponse(category.Id, category.Name, category.IsActive);
        return CreatedAtAction(nameof(GetAll), response);
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
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"Category {id} not found.");

        var name = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Errors = { ["name"] = ["Name must not be empty or whitespace."] }
            });
        }

        category.Name = name;
        await _db.SaveChangesAsync();

        return Ok(new CategoryResponse(category.Id, category.Name, category.IsActive));
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
        var category = await _db.Categories.FindAsync(id);
        if (category is null)
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Not Found",
                detail: $"Category {id} not found.");

        category.IsActive = request.IsActive;
        await _db.SaveChangesAsync();

        _logger.LogInformation("Category {CategoryId} active state changed to {IsActive}",
            category.Id, category.IsActive);

        return Ok(new CategoryResponse(category.Id, category.Name, category.IsActive));
    }
}
