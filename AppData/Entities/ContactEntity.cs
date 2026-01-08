using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Entities
{
    public class ContactEntity
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    internal class ContactEntityConfiguration : IEntityTypeConfiguration<ContactEntity>
    {
        public void Configure(EntityTypeBuilder<ContactEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Message)
                .IsRequired()
                .HasMaxLength(5000);
        }
    }
}
