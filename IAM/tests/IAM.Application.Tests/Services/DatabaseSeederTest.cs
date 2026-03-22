using IAM.Application.Services;
using IAM.Domain.Repositories;
using IAM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IAM.Application.Tests.Services;

public class DatabaseSeederTest
{
   [Fact]
   public async Task SeedAsync_ShouldCreateRockBandsAndMembers()
   {
      // Arrange
      var services = new ServiceCollection();

      // Use in-memory database - Nome único por teste para evitar contaminação
      services.AddDbContext<IamDbContext>(options =>
          options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

      // Register Unit of Work (Essencial para o Seeder)
      services.AddScoped<IUnitOfWork, Infrastructure.Repositories.UnitOfWork>();

      // Register repositories
      services.AddScoped<Domain.Repositories.ICustomerRepository, Infrastructure.Repositories.CustomerRepository>();
      services.AddScoped<Domain.Repositories.IUserRepository, Infrastructure.Repositories.UserRepository>();

      // Register Seeder
      services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

      var serviceProvider = services.BuildServiceProvider();

      // Act
      using var scope = serviceProvider.CreateScope();
      var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
      await seeder.SeedAsync();

      // Assert
      var dbContext = scope.ServiceProvider.GetRequiredService<IamDbContext>();

      // 4 Bands: Beatles, Rolling Stones, Guns, Metallica
      var customers = await dbContext.Customers.ToListAsync();
      Assert.Equal(4, customers.Count);

      // 16 members (4 per band)
      var users = await dbContext.Users.ToListAsync();
      Assert.Equal(16, users.Count);

      // Verify Bands (Customers)
      Assert.Contains(customers, c => c.Code == "BEATLES");
      Assert.Contains(customers, c => c.Code == "STONES");
      Assert.Contains(customers, c => c.Code == "GNR");
      Assert.Contains(customers, c => c.Code == "METALLICA");

      // Verify Specific Members (Users)
      Assert.Contains(users, u => u.Email == "john@beatles.com");
      Assert.Contains(users, u => u.Email == "mick@stones.com");
      Assert.Contains(users, u => u.Email == "slash@gnr.com");
      Assert.Contains(users, u => u.Email == "james@metallica.com");
   }
}