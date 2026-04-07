using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace IAM.Infrastructure.Repositories;

public class CustomerRepository(IamDbContext dbContext) : BaseRepository<Customer>(dbContext), ICustomerRepository
{
   public async Task<Customer?> GetByCodeAsync(string code)
   {
      return await dbContext.Customers.SingleOrDefaultAsync(c => c.Code == code);
   }
}