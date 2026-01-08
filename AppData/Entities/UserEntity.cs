using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppData.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public RoleEntity? Role { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);                

            builder.Property(u => u.Id)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(u => u.Email)
                .IsRequired();

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Password)
                .IsRequired();

            builder.Property(u => u.RoleId)
                .IsRequired();

            builder.HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(u => u.Enabled)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired();
        }
    }
}
