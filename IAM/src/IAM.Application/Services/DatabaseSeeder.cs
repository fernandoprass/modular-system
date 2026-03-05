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
         var user1 = User.Create(
            "John Doe",
            "john.doe@acme.com",
            Argon2.Hash("password123"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440000")
         );

         var user2 =  User.Create(
            "Jane Smith",
            "jane.smith@techstart.com",
            Argon2.Hash("password123"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440001")
         );

         var user3 = User.Create(
            "Bob Johnson",
            "bob.johnson@acme.com",
            Argon2.Hash("password123"),
            Guid.Parse("550e8400-e29b-41d4-a716-446655440000")
         );

         await _unitOfWork.Users.AddAsync(user1);
         await _unitOfWork.Users.AddAsync(user2);
         await _unitOfWork.Users.AddAsync(user3);
      }

      await _unitOfWork.SaveChangesAsync();
   }
}