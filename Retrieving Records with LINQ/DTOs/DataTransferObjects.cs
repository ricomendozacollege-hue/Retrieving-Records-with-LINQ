namespace Retrieving_Records_with_LINQ.DTOs
{
    public class DataTransferObjects
    {
        public record CategoryDto(int Id, string Name);
        public record ProductDto(int Id, string Name, decimal Price, int Stock, int CategoryId, CategoryDto? Category);

        public record CreateCategoryDto(string Name);
        public record CreateProductDto(string Name, decimal Price, int Stock, int CategoryId);

        public record StockSummaryDto(int TotalStock, decimal AveragePrice, int TotalProducts);
    }
}
