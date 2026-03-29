using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories
{
   public class ParameterCustomerRepository(IamDbContext context) : BaseRepository<ParameterCustomer>(context), IParameterCustomerRepository
   {
      public async Task<ParameterCustomer?> GetByParameterAndCustomerAsync(Guid parameterId, Guid customerId)
      {
         return await _dbSet.FirstOrDefaultAsync(pc => pc.ParameterId == parameterId && pc.CustomerId == customerId);
      }
   }
}
