using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Infrastructure.Configurations;

namespace IAM.Infrastructure.Configurations;

public class RoleConfiguration : BaseConfiguration<Role>
{
   public override void Configure(EntityTypeBuilder<Role> builder)
   {
      base.Configure(builder);

      builder.Property(r => r.Name).IsRequired().HasMaxLength(100);
      builder.Property(r => r.CustomerId);
      builder.Property(r => r.IsDefault).IsRequired().HasDefaultValue(false);
      builder.Property(r => r.CreatedAt).IsRequired();
      builder.Property(r => r.UpdatedAt);

      builder.HasIndex(r => new { r.Name, r.CustomerId }).IsUnique();

      builder.HasMany(r => r.RoleFeatures)
         .WithOne(rf => rf.Role)
         .HasForeignKey(rf => rf.RoleId)
         .OnDelete(DeleteBehavior.Cascade);

      builder.HasMany(r => r.UserRoles)
         .WithOne(ur => ur.Role)
         .HasForeignKey(ur => ur.RoleId)
         .OnDelete(DeleteBehavior.Restrict);
   }
}
