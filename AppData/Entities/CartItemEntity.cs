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
    public class CartItemEntity
    {
        public int Id { get; set; }        
        public int UserId { get; set; }
        public UserEntity? User { get; set; }        
        public int ProductId { get; set; }
        public ProductEntity? Product { get; set; }        
        public byte Quantity { get; set; }        
        public DateTime CreatedAt { get; set; }
    }

    internal class CartItemEntityConfiguration : IEntityTypeConfiguration<CartItemEntity>
    {
        public void Configure(EntityTypeBuilder<CartItemEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UserId)
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.ProductId)
                .IsRequired();

            builder.HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.Property (p => p.CreatedAt)
                .IsRequired();
        }
    }
}
