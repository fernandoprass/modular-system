using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAM.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
   public void Configure(EntityTypeBuilder<User> builder)
   {
      builder.HasKey(u => u.Id);
      builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
      builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
      builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
      builder.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(128);
      builder.Property(u => u.CreatedAt).IsRequired();
      builder.Property(u => u.UpdatedAt);
      builder.Property(u => u.CustomerId).IsRequired();

      builder.HasIndex(u => u.Email).IsUnique();

      // Foreign key
      builder.HasOne(u => u.Customer)
             .WithMany(c => c.Users)
             .HasForeignKey(u => u.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);
   }
}