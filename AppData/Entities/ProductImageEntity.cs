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
    public class ProductImageEntity
    {        
        public int Id { get; set; }        
        public int ProductId { get; set; }
        public ProductEntity? Product { get; set; }
        [DataType(DataType.Url)]
        public string Url { get; set; } = null!;        
        public DateTime CreatedAt { get; set; }
    }

    internal class ProductImageEntityConfiguration : IEntityTypeConfiguration<ProductImageEntity>
    {
        public void Configure(EntityTypeBuilder<ProductImageEntity> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.ProductId)
                .IsRequired();

            builder.HasOne(p => p.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(p => p.Url)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
