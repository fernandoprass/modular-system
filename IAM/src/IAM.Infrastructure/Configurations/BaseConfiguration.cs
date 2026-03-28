using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
{
   public virtual void Configure(EntityTypeBuilder<T> builder)
   {
      builder.HasKey(x => x.Id);

      builder.Property(x => x.CreatedAt).IsRequired();
      builder.Property(x => x.CreatedBy).IsRequired(false);
      builder.Property(x => x.UpdatedAt);
      builder.Property(x => x.UpdatedBy).IsRequired(false);
   }
}