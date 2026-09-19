using Retrieving_Records_with_LINQ.Models;
using Retrieving_Records_with_LINQ.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Retrieving_Records_with_LINQ
{ 

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Product> Products { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

      
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Books" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop", Price = 999.99m, Stock = 10, CategoryId = 1 },
                new Product { Id = 2, Name = "Smartphone", Price = 699.99m, Stock = 20, CategoryId = 1 },
                new Product { Id = 3, Name = "Programmer Book", Price = 19.99m, Stock = 50, CategoryId = 2 }
            );
        }
    }
}
