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
    public class NewspaperEntity
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    internal class NewspaperEntityConfiguration : IEntityTypeConfiguration<NewspaperEntity>
    {
        public void Configure(EntityTypeBuilder<NewspaperEntity> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .IsRequired();
        }
    }
}
