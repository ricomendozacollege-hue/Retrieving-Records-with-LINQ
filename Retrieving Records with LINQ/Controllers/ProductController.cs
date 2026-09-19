using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retrieving_Records_with_LINQ.DTOs;
using Retrieving_Records_with_LINQ.Extentions;
using Retrieving_Records_with_LINQ.Models;
using static Retrieving_Records_with_LINQ.DTOs.DataTransferObjects;

namespace Retrieving_Records_with_LINQ.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return NotFound($"Product with ID {id} not found.");
        return Ok(product.ToDto());
    }


    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .ToListAsync();

        return Ok(products.Select(p => p.ToDto()));
    }


    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest("Search query cannot be empty.");

        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => EF.Functions.Like(p.Name, $"%{name}%"))
            .AsNoTracking()
            .ToListAsync();

        return Ok(products.Select(p => p.ToDto()));
    }


    [HttpGet("price-range")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
    {
        if (minPrice > maxPrice)
            return BadRequest("minPrice cannot be greater than maxPrice.");

        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .AsNoTracking()
            .ToListAsync();

        return Ok(products.Select(p => p.ToDto()));
    }


    [HttpGet("summary")]
    public async Task<ActionResult<StockSummaryDto>> GetProductsSummary()
    {
        var hasProducts = await _context.Products.AnyAsync();
        if (!hasProducts) return Ok(new StockSummaryDto(0, 0, 0));

        var totalStock = await _context.Products.SumAsync(p => p.Stock);
        var averagePrice = await _context.Products.AverageAsync(p => p.Price);
        var totalCount = await _context.Products.CountAsync();

        return Ok(new StockSummaryDto(totalStock, Math.Round(averagePrice, 2), totalCount));
    }


    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
        if (!categoryExists) return BadRequest("Invalid CategoryId.");

        var product = new Product
        {
            Name = dto.Name,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return Ok(product.ToDto());
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound($"Product with ID {id} not found.");

        product.Name = dto.Name;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;

        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound($"Product with ID {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}