using Retrieving_Records_with_LINQ.DTOs;
using Retrieving_Records_with_LINQ.Models;
using static Retrieving_Records_with_LINQ.DTOs.DataTransferObjects;

namespace Retrieving_Records_with_LINQ.Extentions;

public static class MappingExtension
{
    public static CategoryDto ToDto(this Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        return new CategoryDto(category.Id, category.Name);
    }

    public static ProductDto ToDto(this Product product)
    {
        ArgumentNullException.ThrowIfNull(product);
        return new ProductDto(
            product.Id,
            product.Name,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Category != null ? product.Category.ToDto() : null
        );
    }
}
