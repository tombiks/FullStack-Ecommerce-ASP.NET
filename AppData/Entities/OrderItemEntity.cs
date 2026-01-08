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
    public class OrderItemEntity
    {        
        public int Id { get; set; }        
        public int OrderId { get; set; }
        public OrderEntity? Order { get; set; }        
        public int ProductId { get; set; }
        public ProductEntity? Product { get; set; }        
        public byte Quantity { get; set; }
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get; set; }       
        public DateTime CreatedAt { get; set; }
    }

    internal class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItemEntity>
    {
        public void Configure(EntityTypeBuilder<OrderItemEntity> builder)
        {
            builder.HasKey(o  => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(o => o.OrderId)
                .IsRequired();

            builder.HasOne(o => o.Order)
                .WithMany()
                .HasForeignKey(o => o.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.Property(o => o.ProductId)
                .IsRequired();

            builder.HasOne(o => o.Product)
                .WithMany()
                .IsRequired()
                .HasForeignKey(o => o.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(o =>o.Quantity)
                .IsRequired();

            builder.Property(o => o.UnitPrice)
                .IsRequired();

            builder.Property(o => o.CreatedAt)
                .IsRequired();
        }
    }
}
