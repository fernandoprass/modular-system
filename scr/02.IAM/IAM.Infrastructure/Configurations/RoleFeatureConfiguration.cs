using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAM.Infrastructure.Configurations;

public class RoleFeatureConfiguration : IEntityTypeConfiguration<RoleFeature>
{
   public void Configure(EntityTypeBuilder<RoleFeature> builder)
   {
      builder.HasKey(rf => new { rf.RoleId, rf.FeatureId });

      builder.HasOne(rf => rf.Role)
         .WithMany(r => r.RoleFeatures)
         .HasForeignKey(rf => rf.RoleId);

      builder.HasOne(rf => rf.Feature)
         .WithMany(f => f.RoleFeatures)
         .HasForeignKey(rf => rf.FeatureId);
   }
}
