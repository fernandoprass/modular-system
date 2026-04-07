using IAM.Domain.Repositories;
using Shared.Domain.Interfaces;

namespace IAM.Domain.Interfaces;

public interface IIamUnitOfWork : IUnitOfWork
{
   ICustomerRepository Customers { get; }
   IRoleRepository Roles { get; }
   IUserRepository Users { get; }
}