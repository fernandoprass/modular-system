using IAM.Domain;
using IAM.Domain.Entities;
using IAM.Domain.Enums;
using IAM.Domain.Interfaces;
using Isopoh.Cryptography.Argon2;
using Shared.Domain;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Shared.Domain.Interfaces;

namespace IAM.Infrastructure;

public interface IDatabaseSeeder
{
   Task SeedAsync();
}

public class DatabaseSeeder : IDatabaseSeeder
{
   private readonly ISharedUnitOfWork _sharedUnitOfWork;
   private readonly IIamUnitOfWork _iamUnitOfWork;
   private readonly IParameterRepository _parameterRepository;
   private const string DefaultPassword = "Password123!";

   public DatabaseSeeder(
      IParameterRepository parameterRepository,
      ISharedUnitOfWork sharedUnitOfWork,
      IIamUnitOfWork iamUnitOfWork)
   {
      _parameterRepository = parameterRepository;
      _sharedUnitOfWork = sharedUnitOfWork;
      _iamUnitOfWork = iamUnitOfWork;
   }

   public async Task SeedAsync()
   {
      //await SeedAdminCustomerAsync();
     // await SeedScientistsCustomerAsync();

      //await SeedParamentersAsync();
   }

   private async Task SeedAdminCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
      if (await _iamUnitOfWork.Customers.ExistsAsync(customerId)) return;

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

      await _iamUnitOfWork.Customers.AddAsync(customer);
      await _iamUnitOfWork.Users.AddAsync(superUser);
      await _iamUnitOfWork.Users.AddAsync(User.Create("Internal Support", "support@saas.com", passwordHash, customerId));
      await _iamUnitOfWork.SaveChangesAsync();
   }
    
   private async Task SeedScientistsCustomerAsync()
   {
      var customerId = Guid.Parse("00000000-0000-0000-0000-000000000002");
      if (await _iamUnitOfWork.Customers.ExistsAsync(customerId)) return;

      var customer = new Customer
      {
         Id = customerId,
         Name = "Computing Pioneers Society",
         Code = "SCIENTISTS",
         Type = CustomerType.Company,
         Description = "Foundation of modern Computer Science",
         CreatedAt = DateTime.UtcNow
      };

      await _iamUnitOfWork.Customers.AddAsync(customer);

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
         await _iamUnitOfWork.Users.AddAsync(User.Create(name, email, passwordHash, customerId));
      }
      await _iamUnitOfWork.SaveChangesAsync();
   }

   //todo fix using ParameterService instead of repository
   //private async Task SeedParamentersAsync()
   //{
   //   await AddParameter(IamParam.Security.PasswordExpireTime, ParameterType.Integer,
   //                      "Password expiration time", "Password expiration time, in days.", "60", false, false);

   //   await _iamUnitOfWork.SaveChangesAsync();
   //}

   //private async Task AddParameter(
   //   string key,
   //   ParameterType type,
   //   string title,
   //   string description,
   //   string value,
   //   bool isCustomerEditable,
   //   bool isVisible)
   //{
   //   var param = await _parameterRepository.GetByKeyAsync(key);
   //   if (param is null)
   //   {
   //      var parameter = CreateParameter(key, type, title, description, value, isCustomerEditable, isVisible);
   //      await _sharedUnitOfWork.Parameters.AddAsync(parameter);
   //   }
   //}

   //private static Parameter CreateParameter(
   //   string key, 
   //   ParameterType type, 
   //   string title, 
   //   string description, 
   //   string value, 
   //   ParameterOverrideType overrideType,
   //   bool isVisible)
   //{
   //   var parameterKey = new ParameterKey(key);

   //   return Parameter.Create(
   //      parameterKey.Module,
   //      parameterKey.Group,
   //      parameterKey.Name,
   //      title,
   //      description,
   //      type,
   //      value,
   //      overrideType: overrideType,
   //      isVisible: isVisible);
   //}
}