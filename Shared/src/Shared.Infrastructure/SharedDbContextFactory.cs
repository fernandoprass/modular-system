using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Shared.Infrastructure;

public class SharedDbContextFactory : IDesignTimeDbContextFactory<SharedDbContext>
{
   public SharedDbContext CreateDbContext(string[] args)
   {
      string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", optional: false)
          .AddJsonFile($"appsettings.{environment}.json", optional: true)
          .Build();

      var connectionString = configuration.GetConnectionString("SharedDb");

      if (string.IsNullOrEmpty(connectionString))
      {
         throw new InvalidOperationException("Could not find a connection string named 'SharedConnection' in appsettings.json");
      }

      var optionsBuilder = new DbContextOptionsBuilder<SharedDbContext>();
      optionsBuilder.UseNpgsql(connectionString);

      return new SharedDbContext(optionsBuilder.Options);
   }
}