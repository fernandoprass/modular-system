using IAM.Core.Services;
using IAM.Domain;
using IAM.Domain.Repositories;
using IAM.Domain.QueryRepositories;
using IAM.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IAM.API.Testing;

public class DatabaseSeederTest
{
    [Fact]
    public async Task SeedAsync_ShouldCreateSampleData()
    {
        // Arrange
        var services = new ServiceCollection();

        // Use in-memory database for testing
        services.AddDbContext<IamDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        // Register repositories
        services.AddScoped<IAM.Domain.Repositories.ICustomerRepository, IAM.Infrastructure.Repositories.CustomerRepository>();
        services.AddScoped<IAM.Domain.Repositories.IUserRepository, IAM.Infrastructure.Repositories.UserRepository>();
        services.AddScoped<IAM.Domain.QueryRepositories.ICustomerQueryRepository, IAM.Infrastructure.QueryRepositories.CustomerQueryRepository>();
        services.AddScoped<IAM.Domain.Repositories.IUserQueryRepository, IAM.Infrastructure.Repositories.UserQueryRepository>();

        // Register services
        services.AddScoped<ICustomerService, IAM.Core.Services.CustomerService>();
        services.AddScoped<IUserService, IAM.Core.Services.UserService>();
        services.AddScoped<IAuthService, IAM.Core.Services.AuthService>();
        services.AddScoped<IDatabaseSeeder, IAM.Core.Services.DatabaseSeeder>();

        var serviceProvider = services.BuildServiceProvider();

        // Act
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
        await seeder.SeedAsync();

        // Assert
        var dbContext = scope.ServiceProvider.GetRequiredService<IamDbContext>();
        var customers = await dbContext.Customers.ToListAsync();
        var users = await dbContext.Users.ToListAsync();

        Assert.Equal(2, customers.Count);
        Assert.Equal(3, users.Count);

        // Verify customer names
        Assert.Contains(customers, c => c.Name == "Acme Corporation");
        Assert.Contains(customers, c => c.Name == "TechStart Inc");

        // Verify user emails
        Assert.Contains(users, u => u.Email == "admin@acme.com");
        Assert.Contains(users, u => u.Email == "user@acme.com");
        Assert.Contains(users, u => u.Email == "admin@techstart.com");
    }
}