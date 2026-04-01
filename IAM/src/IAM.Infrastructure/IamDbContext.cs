using IAM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using IAM.Infrastructure.Configurations;

namespace IAM.Infrastructure;

public class IamDbContext : DbContext
{
   public IamDbContext(DbContextOptions<IamDbContext> options) : base(options)
   {
   }

   public DbSet<Customer> Customers { get; set; }
   public DbSet<Feature> Features { get; set; }
   public DbSet<Role> Roles { get; set; }
   public DbSet<RoleFeature> RoleFeatures { get; set; }
   public DbSet<User> Users { get; set; }
   public DbSet<UserRole> UserRoles { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.ApplyConfiguration(new CustomerConfiguration());
      modelBuilder.ApplyConfiguration(new UserConfiguration());
      modelBuilder.ApplyConfiguration(new RoleConfiguration());
      modelBuilder.ApplyConfiguration(new FeatureConfiguration());
      modelBuilder.ApplyConfiguration(new RoleFeatureConfiguration());
      modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
   }
}