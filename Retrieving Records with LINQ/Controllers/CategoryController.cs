using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retrieving_Records_with_LINQ.DTOs;
using Retrieving_Records_with_LINQ.Extentions;
using Retrieving_Records_with_LINQ.Models;
using static Retrieving_Records_with_LINQ.DTOs.DataTransferObjects;

namespace Retrieving_Records_with_LINQ.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        var categories = await _context.Categories.AsNoTracking().ToListAsync();
        return Ok(categories.Select(c => c.ToDto()));
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");
        return Ok(category.ToDto());
    }

    
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Category name is required.");

        var category = new Category { Name = dto.Name };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category.ToDto());
    }

  
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CreateCategoryDto dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");

        category.Name = dto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found.");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}