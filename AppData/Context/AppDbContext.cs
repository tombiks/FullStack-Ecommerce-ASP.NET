using AppData.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<CartItemEntity> CartItems { get; set; }
        public DbSet<CategoryEntity> Categories { get; set; }
        public DbSet<OrderEntity> Orders { get; set; }
        public DbSet<OrderItemEntity> OrderItems { get; set; }
        public DbSet<ProductCommentEntity> ProductComments { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<ProductImageEntity> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            //SeedData
            //Roles
            modelBuilder.Entity<RoleEntity>().HasData(
                new RoleEntity { Id = 1, Name="Admin", CreatedAt = DateTime.Now },
                new RoleEntity { Id = 2, Name="Seller", CreatedAt = DateTime.Now },
                new RoleEntity { Id = 3, Name="Buyer", CreatedAt = DateTime.Now }
                );

            //Users
            modelBuilder.Entity<UserEntity>().HasData(
                new UserEntity { Id = 1, Email = "admin@admin.net", FirstName = "admin", LastName = "admin" , Password = "admin123", Enabled = true, RoleId =  1, CreatedAt = DateTime.Now},
                new UserEntity { Id = 2, Email = "seller@selller.net", FirstName = "seller", LastName = "seller", Password = "seller123", Enabled = true, RoleId = 2, CreatedAt = DateTime.Now },
                new UserEntity { Id = 3, Email = "buyer@buyer.net", FirstName = "buyer", LastName = "buyer", Password = "buyer123", Enabled = true, RoleId = 3, CreatedAt = DateTime.Now }
                );

            //Categories
            modelBuilder.Entity<CategoryEntity>().HasData(
                new CategoryEntity { Id = 1, Name = "Elektronik", Color = "#3498db", IconCssClass = "fas fa-laptop", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 2, Name = "Moda", Color = "#e74c3c", IconCssClass = "fas fa-tshirt", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 3, Name = "Ev & Yaşam", Color = "#2ecc71", IconCssClass = "fas fa-home", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 4, Name = "Spor & Outdoor", Color = "#f39c12", IconCssClass = "fas fa-running", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 5, Name = "Kitap & Kırtasiye", Color = "#9b59b6", IconCssClass = "fas fa-book", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 6, Name = "Oyuncak & Hobi", Color = "#e91e63", IconCssClass = "fas fa-gamepad", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 7, Name = "Kozmetik & Kişisel Bakım", Color = "#ff6b6b", IconCssClass = "fas fa-spa", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 8, Name = "Gıda & İçecek", Color = "#4ecdc4", IconCssClass = "fas fa-utensils", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 9, Name = "Otomotiv & Aksesuar", Color = "#95a5a6", IconCssClass = "fas fa-car", CreatedAt = DateTime.Now },
                new CategoryEntity { Id = 10, Name = "Sağlık & Medikal", Color = "#27ae60", IconCssClass = "fas fa-heartbeat", CreatedAt = DateTime.Now }
                );

            // Products
            var products = new List<ProductEntity>();
            int productId = 1;
            for (int i = 1; i <= 10; i++) // Categories
            {
                for (int j = 0; j < 3; j++) // Products per category
                {
                    products.Add(new ProductEntity
                    {
                        Id = productId,
                        SellerId = 2,
                        CategoryId = i,
                        Name = $"Category {i} Product {j + 1}",
                        Price = (productId * 10),
                        Details = $"Details for product {productId}",
                        StockAmount = (byte)(productId * 5),
                        CreatedAt = DateTime.Now,
                        Enabled = true
                    });
                    productId++;
                }
            }
            modelBuilder.Entity<ProductEntity>().HasData(products);

            // Product Images
            var productImages = new List<ProductImageEntity>();
            var imageFiles = new[] { "product-1.jpg", "product-2.jpg", "product-3.jpg", "product-4.jpg", "product-5.jpg", "product-6.jpg", "product-7.jpg", "product-8.jpg", "product-9.jpg", "product-10.jpg", "product-11.jpg", "product-12.jpg" };
            for (int i = 1; i <= 30; i++)
            {
                productImages.Add(new ProductImageEntity
                {
                    Id = i,
                    ProductId = i,
                    Url = $"/uploads/products/{imageFiles[(i - 1) % 12]}",
                    CreatedAt = DateTime.Now
                });
            }
            modelBuilder.Entity<ProductImageEntity>().HasData(productImages);

            // Product Comments
            var comments = new List<ProductCommentEntity>();
            int commentId = 1;
            for (int i = 1; i <= 30; i++) // Products
            {
                for (int j = 0; j < 3; j++) // Comments per product
                {
                    comments.Add(new ProductCommentEntity
                    {
                        Id = commentId,
                        ProductId = i,
                        UserId = 3,
                        Text = $"This is comment {j + 1} for product {i}",
                        StarCount = (byte)((commentId % 5) + 1),
                        IsConfirmed = true,
                        CreatedAt = DateTime.Now
                    });
                    commentId++;
                }
            }
            modelBuilder.Entity<ProductCommentEntity>().HasData(comments);
        }

    }
}
