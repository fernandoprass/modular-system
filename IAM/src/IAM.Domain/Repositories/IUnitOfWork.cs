namespace IAM.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICustomerRepository Customers { get; }
    IRoleRepository Roles { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
