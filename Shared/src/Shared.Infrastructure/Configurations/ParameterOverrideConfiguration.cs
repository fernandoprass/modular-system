using Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Configurations
{
   public class ParameterOverrideConfiguration : BaseConfiguration<ParameterOverride>
   {
      public override void Configure(EntityTypeBuilder<ParameterOverride> builder)
      {
         base.Configure(builder);
         builder.ToTable("ParameterOverrides", "shared");

         builder.Property(p => p.Value).IsRequired().HasColumnType("text");
         builder.Property(pc => pc.OwnerId).IsRequired();
         builder.Property(pc => pc.OwnerType).IsRequired().HasMaxLength(50);
         builder.Property(pc => pc.ParameterId).IsRequired();

         builder.HasOne<Parameter>()
                .WithMany(p => p.ParameterOwners)
                .HasForeignKey(pc => pc.ParameterId)
                .OnDelete(DeleteBehavior.Cascade);
      }
   }
}
