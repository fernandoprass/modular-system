using Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Infrastructure.Configurations
{
   public class ParameterCustomerConfiguration : BaseConfiguration<ParameterCustomer>
   {
      public override void Configure(EntityTypeBuilder<ParameterCustomer> builder)
      {
         base.Configure(builder);
         builder.ToTable("ParameterCustomers", "shared");

         builder.Property(p => p.Value).IsRequired().HasColumnType("text");
         builder.Property(pc => pc.CustomerId).IsRequired();
         builder.Property(pc => pc.ParameterId).IsRequired();

         builder.HasOne<Parameter>()
                .WithMany(p => p.ParameterCustomers)
                .HasForeignKey(pc => pc.ParameterId)
                .OnDelete(DeleteBehavior.Cascade);
      }
   }
}
