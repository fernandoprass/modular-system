namespace IAM.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICustomerRepository Customers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
