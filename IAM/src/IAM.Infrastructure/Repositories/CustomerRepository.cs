using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
   public CustomerRepository(IamDbContext context) : base(context)
   {
   }

   public async Task<Customer?> GetByCodeAsync(string code)
   {
      return await _context.Customers.SingleOrDefaultAsync(c => c.Code == code);
   }

   public void Update(Customer customer)
   {
      _dbSet.Update(customer);
   }
}