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
    public class OrderEntity
    {        
        public int Id { get; set; }        
        public int UserId { get; set; }
        public UserEntity? User { get; set; }                 
        public DateTime CreatedAt { get; set; }
    }

    internal class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            
            builder.Property(o => o.CreatedAt)
                .IsRequired();
        }
    }
}
