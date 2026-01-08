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
    public class CategoryEntity
    {        
        public int Id { get; set; }        
        public string Name { get; set; } = null!;        
        public string Color { get; set; } = null!;        
        public string IconCssClass { get; set; } = null!;        
        public DateTime CreatedAt { get; set; }
    }

    internal class CategoryEntityConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Color)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(c => c.IconCssClass)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.CreatedAt)
                .IsRequired();
        }
    }
}
