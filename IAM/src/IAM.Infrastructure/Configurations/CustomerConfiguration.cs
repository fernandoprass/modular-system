using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAM.Infrastructure.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
   public void Configure(EntityTypeBuilder<Customer> builder)
   {
      builder.HasKey(c => c.Id);
      builder.Property(c => c.Type).IsRequired();
      builder.Property(c => c.Code).IsRequired().HasMaxLength(25);
      builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
      builder.Property(c => c.Description).HasMaxLength(1000);
      builder.Property(c => c.IsActive).IsRequired().HasDefaultValue(true);
      builder.Property(c => c.IsMaster).IsRequired().HasDefaultValue(false);
      builder.Property(c => c.CreatedAt).IsRequired();
      builder.Property(c => c.UpdatedAt);
   }
}