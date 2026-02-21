using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Isopoh.Cryptography.Argon2;

namespace IAM.Application.Services;

public interface IDatabaseSeeder
{
   Task SeedAsync();
}

public class DatabaseSeeder : IDatabaseSeeder
{
   private readonly IUnitOfWork _unitOfWork;

   public DatabaseSeeder(IUnitOfWork unitOfWork)
   {
      _unitOfWork = unitOfWork;
   }

   public async Task SeedAsync()
   {
      // Seed customers
      if (!await _unitOfWork.Customers.ExistsAsync(Guid.Parse("550e8400-e29b-41d4-a716-446655440000")))
      {
         var customer1 = new Customer
         {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            Type = CustomerType.Company,
            Code = "ACME",
            Name = "Acme Corporation",
            Description = "Leading technology company",
            CreatedAt = DateTime.UtcNow
         };

         var customer2 = new Customer
         {
            Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            Type = CustomerType.Company,
            Code = "TECHSTART",
            Name = "TechStart Inc",
            Description = "Innovative startup",
            CreatedAt = DateTime.UtcNow
         };

         await _unitOfWork.Customers.AddAsync(customer1);
         await _unitOfWork.Customers.AddAsync(customer2);
      }

      // Seed users
      if (!await _unitOfWork.Users.ExistsAsync(Guid.Parse("550e8400-e29b-41d4-a716-446655440010")))
      {
         var user1 = new User
         {
            Id = Guid.CreateVersion7(),
            Name = "John Doe",
            Email = "john.doe@acme.com",
            PasswordHash = Argon2.Hash("password123"),
            CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            CreatedAt = DateTime.UtcNow
         };

         var user2 = new User
         {
            Id = Guid.CreateVersion7(),
            Name = "Jane Smith",
            Email = "jane.smith@techstart.com",
            PasswordHash = Argon2.Hash("password123"),
            CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440001"),
            CreatedAt = DateTime.UtcNow
         };

         var user3 = new User
         {
            Id = Guid.CreateVersion7(),
            Name = "Bob Johnson",
            Email = "bob.johnson@acme.com",
            PasswordHash = Argon2.Hash("password123"),
            CustomerId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
            CreatedAt = DateTime.UtcNow
         };

         await _unitOfWork.Users.AddAsync(user1);
         await _unitOfWork.Users.AddAsync(user2);
         await _unitOfWork.Users.AddAsync(user3);
      }

      await _unitOfWork.SaveChangesAsync();
   }
}