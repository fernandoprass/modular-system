using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Repositories
{
   public class ParameterCustomerRepository(SharedDbContext dbContext) 
      : BaseRepository<ParameterCustomer>(dbContext), IParameterCustomerRepository
   {
      public async Task<ParameterCustomer?> GetByParameterAndCustomerAsync(Guid parameterId, Guid customerId)
      {
         return await _dbSet.FirstOrDefaultAsync(pc => pc.ParameterId == parameterId && pc.CustomerId == customerId);
      }
   }
}
