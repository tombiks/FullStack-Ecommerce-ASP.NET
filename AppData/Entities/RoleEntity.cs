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
    public class RoleEntity
    {        
        public int Id { get; set; }        
        public string Name { get; set; } = null!;        
        public DateTime CreatedAt { get; set; }
    }

    internal class RoleEntityConfiguration : IEntityTypeConfiguration<RoleEntity>
    {
        public void Configure(EntityTypeBuilder<RoleEntity> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(r => r.CreatedAt)
                .IsRequired();
        }
    }
}
