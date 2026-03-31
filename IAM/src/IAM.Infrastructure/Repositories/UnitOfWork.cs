using IAM.Domain.Entities;
using IAM.Domain.Interfaces;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
   private readonly IamDbContext _dbContext;
   public ICustomerRepository Customers { get; }
   public IUserContext _userContext;
   public IUserRepository Users { get; }
   public IRoleRepository Roles { get; }
   public IParameterRepository Parameters { get; }
   public IParameterCustomerRepository ParameterCustomers { get; }

   public UnitOfWork(IamDbContext dbContext,
      IUserContext userContext,
      IUserRepository userRepository,
      ICustomerRepository customerRepository,
      IRoleRepository roleRepository,
      IParameterRepository parameterRepository,
      IParameterCustomerRepository parameterCustomerRepository)
   {
      _dbContext = dbContext;
      _userContext = userContext;
      Users = userRepository;
      Customers = customerRepository;
      Roles = roleRepository;
      Parameters = parameterRepository;
      ParameterCustomers = parameterCustomerRepository;
   }

   //public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
   //{
   //    return _dbContext.SaveChangesAsync(cancellationToken);
   //}

   public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
   {
      var userId = _userContext.UserId;
      var entries = _dbContext.ChangeTracker.Entries<Entity>();

      foreach (var entry in entries)
      {
         if (entry.State == EntityState.Added)
         {
            entry.Entity.CreatedAt = DateTime.UtcNow;

            if (userId == Guid.Empty && entry.Entity is User newUser)
            {
               entry.Entity.CreatedBy = newUser.Id;
            }
            else
            {
               entry.Entity.CreatedBy = entry.Entity.CreatedBy == Guid.Empty ? userId : entry.Entity.CreatedBy;
            }
         }

         if (entry.State == EntityState.Modified)
         {
            entry.Entity.UpdatedAt = DateTime.UtcNow;
            entry.Entity.UpdatedBy = _userContext.UserId == Guid.Empty ? null : _userContext.UserId;
         }
      }
      return _dbContext.SaveChangesAsync(cancellationToken);
   }

   public void Dispose()
   {
      _dbContext.Dispose();
   }
}
