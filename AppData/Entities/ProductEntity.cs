using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Entities
{
    public class ProductEntity
    {        
        public int Id { get; set; }        
        public int SellerId { get; set; }
        public UserEntity? Seller { get; set; }        
        public int CategoryId { get; set; }
        public CategoryEntity? Category { get; set; }        
        public string Name { get; set; } = null!;        
        public decimal Price { get; set; }        
        public string Details { get; set; } = string.Empty;        
        public byte StockAmount { get; set; }        
        public DateTime CreatedAt { get; set; }       
        public bool Enabled { get; set; } = true;
        public ICollection<ProductImageEntity>? Images { get; set; }
        public ICollection<ProductCommentEntity>? Comments { get; set; }
    }

    internal class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.SellerId)
                .IsRequired();

            builder.HasOne(p => p.Seller)
                .WithMany()
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("money");

            builder.Property(p => p.Details)
                .HasMaxLength(1000);

            builder.Property(p => p.StockAmount)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.Enabled)
                .IsRequired()
                .HasDefaultValue(true);
        }
    }
}
