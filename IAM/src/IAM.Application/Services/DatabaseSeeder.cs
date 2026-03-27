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
   private const string DefaultPassword = "Password123!";

   public DatabaseSeeder(IUnitOfWork unitOfWork)
   {
      _unitOfWork = unitOfWork;
   }

   public async Task SeedAsync()
   {
      await SeedAdminCustomerAsync();
      await SeedScientistsCustomerAsync();

      await _unitOfWork.SaveChangesAsync();
   }

   private async Task SeedAdminCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      if (await _unitOfWork.Customers.ExistsAsync(customerId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = customerId,
         Name = "SaaS Internal Administration",
         Code = "SAASADMIN",
         Type = CustomerType.Company,
         Description = "Internal system management and support",
         IsMaster = true, // Following our Master Customer rule
         CreatedAt = DateTime.UtcNow
      });

      var passwordHash = Argon2.Hash(DefaultPassword);

      var superUser = User.Create("System Root", "admin@saas.com", passwordHash, customerId);
      superUser.IsSystemAdmin = true;
      await _unitOfWork.Users.AddAsync(superUser);
      await _unitOfWork.Users.AddAsync(User.Create("Internal Support", "support@saas.com", passwordHash, customerId));
   }

   private async Task SeedScientistsCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000002");
      if (await _unitOfWork.Customers.ExistsAsync(customerId)) return;

      await _unitOfWork.Customers.AddAsync(new Customer
      {
         Id = customerId,
         Name = "Computing Pioneers Society",
         Code = "SCIENTISTS",
         Type = CustomerType.Company,
         Description = "Foundation of modern Computer Science",
         CreatedAt = DateTime.UtcNow
      });

      var passwordHash = Argon2.Hash(DefaultPassword);
      var members = new[]
      {
            ("Alan Turing", "alan.turing@enigma.org"),
            ("Ada Lovelace", "ada.lovelace@analytical.org"),
            ("Grace Hopper", "grace.hopper@cobol.org"),
            ("John von Neumann", "john.vonneumann@architecture.org"),
            ("Claude Shannon", "claude.shannon@entropy.org")
        };

      foreach (var (name, email) in members)
      {
         await _unitOfWork.Users.AddAsync(User.Create(name, email, passwordHash, customerId));
      }
   }
}