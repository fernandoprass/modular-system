using Shared.Domain.Repositories;

namespace Shared.Domain.Interfaces;

public interface ISharedUnitOfWork : IUnitOfWork
{
   IParameterRepository Parameters { get; }
   IParameterCustomerRepository ParameterCustomers { get; }
}