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
    public class ProductCommentEntity
    {        
        public int Id { get; set; }        
        public int ProductId { get; set; }
        public ProductEntity? Product { get; set; }        
        public int UserId { get; set; }
        public UserEntity? User { get; set; }        
        public string Text { get; set; } = null!;        
        public byte StarCount { get; set; }        
        public bool IsConfirmed { get; set; } = false;        
        public DateTime CreatedAt { get; set; }
    }

    internal class ProductCommentEntityConfiguration : IEntityTypeConfiguration<ProductCommentEntity>
    {
        public void Configure(EntityTypeBuilder<ProductCommentEntity> builder)
        {
            builder.HasKey(p  => p.Id);

            builder.Property(p => p.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.ProductId)
                .IsRequired();

            builder.HasOne(p  => p.Product)
                .WithMany(p => p.Comments)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.Property(p => p.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.StarCount)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(p => p.IsConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
