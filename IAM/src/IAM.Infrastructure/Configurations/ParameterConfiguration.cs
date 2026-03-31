using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAM.Infrastructure.Configurations
{
   public class ParameterConfiguration : BaseConfiguration<Parameter>
   {
      public override void Configure(EntityTypeBuilder<Parameter> builder)
      {
         base.Configure(builder);

         builder.Property(p => p.Module).IsRequired().HasMaxLength(100);
         builder.Property(p => p.Group).IsRequired().HasMaxLength(100);
         builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
         builder.Property(p => p.Key).IsRequired().HasMaxLength(402);
         builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
         builder.Property(p => p.Description).IsRequired().HasColumnType("text");
         builder.Property(p => p.Type).IsRequired();
         builder.Property(p => p.Value).IsRequired().HasColumnType("text");
         builder.Property(p => p.ListItems).IsRequired(false).HasColumnType("text");
         builder.Property(p => p.ExternalListEndpoint).IsRequired(false).HasMaxLength(500);
         builder.Property(p => p.IsCustomerEditable).IsRequired().HasDefaultValue(false);
         builder.Property(p => p.IsVisible).IsRequired().HasDefaultValue(true);

         builder.HasIndex(p => new {p.Module, p.Group, p.Name }).IsUnique();

         builder.HasIndex(p => new {p.Key }).IsUnique();
      }
   }
}
