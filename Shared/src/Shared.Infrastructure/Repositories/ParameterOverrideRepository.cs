using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Repositories
{
   public class ParameterOverrideRepository(SharedDbContext dbContext) 
      : BaseRepository<ParameterOverride>(dbContext), IParameterOverrideRepository
   {
      public async Task<ParameterOverride?> GetByParameterIdAndOwnerIdAsync(Guid parameterId, Guid ownerId)
      {
         return await _dbSet.FirstOrDefaultAsync(pc => pc.ParameterId == parameterId && 
                                                       pc.OwnerId == ownerId);
      }
   }
}
