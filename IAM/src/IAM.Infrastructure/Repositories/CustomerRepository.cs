using IAM.Domain.Entities;
using IAM.Domain.Repositories;

namespace IAM.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(IamDbContext context) : base(context)
    {
    }

    public void Update(Customer customer)
    {
        _dbSet.Update(customer);
    }
}