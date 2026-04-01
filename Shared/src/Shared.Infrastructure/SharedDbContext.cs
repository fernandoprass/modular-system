using Shared.Domain.Entities;
using Shared.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure;

public class SharedDbContext : DbContext
{
    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
    {
    }

    public DbSet<Parameter> Parameters { get; set; }
    public DbSet<ParameterCustomer> ParameterCustomers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ParameterConfiguration());
        modelBuilder.ApplyConfiguration(new ParameterCustomerConfiguration());
    }
}
