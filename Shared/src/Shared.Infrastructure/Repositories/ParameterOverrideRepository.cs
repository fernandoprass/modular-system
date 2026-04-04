using Shared.Domain.Entities;
using Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.Repositories
{
   public class ParameterOverrideRepository(SharedDbContext dbContext) 
      : BaseRepository<ParameterOverride>(dbContext), IParameterOverrideRepository
   {
      public async Task<ParameterOverride?> GetByParameterAndOwnerAsync(Guid parameterId, string ownerType, Guid ownerId)
      {
         return await _dbSet.FirstOrDefaultAsync(pc => pc.ParameterId == parameterId && 
                                                       pc.OwnerType == ownerType &&
                                                       pc.OwnerId == ownerId);
      }
   }
}
