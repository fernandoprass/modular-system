using IAM.Domain;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
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
   private readonly IParameterRepository _parameterRepository;
   private const string DefaultPassword = "Password123!";

   public DatabaseSeeder(
      IParameterRepository parameterRepository,
      IUnitOfWork unitOfWork)
   {
      _parameterRepository = parameterRepository;
      _unitOfWork = unitOfWork;
   }

   public async Task SeedAsync()
   {
      //await SeedAdminCustomerAsync();
     // await SeedScientistsCustomerAsync();

      await SeedParamentersAsync();
   }

   private async Task SeedAdminCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      if (await _unitOfWork.Customers.ExistsAsync(customerId)) return;

      var customer = new Customer
      {
         Id = customerId,
         Name = "SaaS Internal Administration",
         Code = "SAASADMIN",
         Type = CustomerType.Company,
         Description = "Internal system management and support",
         IsMaster = true, // Following our Master Customer rule
         CreatedAt = DateTime.UtcNow
      };

      var passwordHash = Argon2.Hash(DefaultPassword);

      var superUser = User.Create("System Root", "admin@saas.com", passwordHash, customerId);
      superUser.IsSystemAdmin = true;
      customer.CreatedBy = superUser.Id;

      await _unitOfWork.Customers.AddAsync(customer);
      await _unitOfWork.Users.AddAsync(superUser);
      await _unitOfWork.Users.AddAsync(User.Create("Internal Support", "support@saas.com", passwordHash, customerId));
      await _unitOfWork.SaveChangesAsync();
   }

   private async Task SeedScientistsCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000002");
      if (await _unitOfWork.Customers.ExistsAsync(customerId)) return;

      var customer = new Customer
      {
         Id = customerId,
         Name = "Computing Pioneers Society",
         Code = "SCIENTISTS",
         Type = CustomerType.Company,
         Description = "Foundation of modern Computer Science",
         CreatedAt = DateTime.UtcNow
      };

      await _unitOfWork.Customers.AddAsync(customer);

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
      await _unitOfWork.SaveChangesAsync();
   }

   private async Task SeedParamentersAsync()
   {
      await AddParameter(Const.Parameter.Group.Security, Const.Parameter.Key.PasswordExpireTime, ParameterType.Integer,
                         "Password expiration time", "Password expiration time, in days.", "60", false, false);
      
      await _unitOfWork.SaveChangesAsync();
   }

   private async Task AddParameter(
      string group,
      string key,
      ParameterType type,
      string name,
      string description,
      string value,
      bool isCustomerEditable,
      bool isVisible)
   {
      var param = await _parameterRepository.GetByGroupAndKeyAsync(group, key);
      if (param is null)
      {
         var parameter = CreateParameter(group, key, type, name, description, value, isCustomerEditable, isVisible);
         await _unitOfWork.Parameters.AddAsync(parameter);
      }
   }

   private static Parameter CreateParameter(
      string group,
      string key, 
      ParameterType type, 
      string name, 
      string description, 
      string value, 
      bool isCustomerEditable,
      bool isVisible)
   {
      return new Parameter
      {
         Id = Guid.CreateVersion7(),
         Group = group,
         Key = key,
         Type = type,
         Name = name,
         Description = description,
         Value = value,
         IsCustomerEditable = isCustomerEditable,
         IsVisible = isVisible
      };
   }
}