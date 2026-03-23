using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAM.Infrastructure.Configurations
{
   public class FeatureConfiguration : IEntityTypeConfiguration<Feature>
   {
      public void Configure(EntityTypeBuilder<Feature> builder)
      {
         builder.HasKey(f => f.Id);
         builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
         builder.Property(f => f.Description).HasMaxLength(250);
         builder.Property(f => f.Group).IsRequired().HasMaxLength(50);
         builder.Property(f => f.CreatedAt).IsRequired();
         builder.Property(f => f.UpdatedAt);

         builder.HasIndex(f => f.Name).IsUnique();

         builder.HasMany(f => f.RoleFeatures)
            .WithOne(rf => rf.Feature)
            .HasForeignKey(rf => rf.FeatureId)
            .OnDelete(DeleteBehavior.Cascade);
      }
   }
}
