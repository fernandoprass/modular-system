using IAM.Application.Services;
using IAM.Domain.Repositories;
using IAM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IAM.Application.Tests.Services;

public class DatabaseSeederTest
{
   [Fact]
   public async Task SeedAsync_ShouldCreateCustomersAndUsers()
   {
      // Arrange
      var services = new ServiceCollection();

      // Use in-memory database - Nome único por teste para evitar contaminação
      services.AddDbContext<IamDbContext>(options =>
          options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

      // Register Unit of Work (Essencial para o Seeder)
      services.AddScoped<IUnitOfWork, Infrastructure.Repositories.UnitOfWork>();

      // Register repositories
      services.AddScoped<ICustomerRepository, Infrastructure.Repositories.CustomerRepository>();
      services.AddScoped<IUserRepository, Infrastructure.Repositories.UserRepository>();

      // Register Seeder
      services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

      var serviceProvider = services.BuildServiceProvider();

      // Act
      using var scope = serviceProvider.CreateScope();
      var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
      await seeder.SeedAsync();

      // Assert
      var dbContext = scope.ServiceProvider.GetRequiredService<IamDbContext>();

      // 2 customers
      var customers = await dbContext.Customers.ToListAsync();
      Assert.Equal(2, customers.Count);

      // 7 members
      var users = await dbContext.Users.ToListAsync();
      Assert.Equal(7, users.Count);

      // Verify Customers
      Assert.Contains(customers, c => c.Code == "SAASADMIN");
      Assert.Contains(customers, c => c.Code == "SCIENTISTS");

      // Verify Specific Members (Users)
      Assert.Contains(users, u => u.Email == "admin@saas.com");
      Assert.Contains(users, u => u.Email == "mick@stones.com");
      Assert.Contains(users, u => u.Email == "slash@gnr.com");
      Assert.Contains(users, u => u.Email == "james@metallica.com");
   }
}