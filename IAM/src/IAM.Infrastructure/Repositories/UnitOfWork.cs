using IAM.Domain.Repositories;

namespace IAM.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly IamDbContext _context;
    public IUserRepository Users { get; }
    public ICustomerRepository Customers { get; }
    public IRoleRepository Roles { get; }

    public UnitOfWork(IamDbContext context, 
       IUserRepository userRepository, 
       ICustomerRepository customerRepository,
       IRoleRepository roleRepository)
    {
        _context = context;
        Users = userRepository;
        Customers = customerRepository;
        Roles = roleRepository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
