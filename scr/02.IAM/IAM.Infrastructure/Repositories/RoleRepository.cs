using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace IAM.Infrastructure.Repositories;

public class RoleRepository(IamDbContext dbContext) : BaseRepository<Role>(dbContext), IRoleRepository
{
   public override async Task<Role?> GetByIdAsync(Guid id)
   {
      return await _dbSet
         .Include(r => r.RoleFeatures)
         .FirstOrDefaultAsync(r => r.Id == id);
   }
}
