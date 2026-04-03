using Shared.Domain.Interfaces;
using Shared.Domain.Repositories;
using Shared.Infrastructure.Repositories;

namespace Shared.Infrastructure.UoW;

/// <summary>
/// SharedUnitOfWork is a concrete implementation of the ISharedUnitOfWork interface, which extends the generic UnitOfWork class.
/// </summary>
public class SharedUnitOfWork : UnitOfWork<SharedDbContext>, ISharedUnitOfWork
{
   private readonly SharedDbContext _dbContext;

   public SharedUnitOfWork(SharedDbContext dbContext, IUserContext userContext)
       : base(dbContext, userContext)
   {
      _dbContext = dbContext;
   }

   public IParameterRepository Parameters => new ParameterRepository(_dbContext);
   public IParameterCustomerRepository ParameterCustomers => new ParameterCustomerRepository(_dbContext);
}