using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories
{
   public class RoleRepository : BaseRepository<Role>, IRoleRepository
   {
      public RoleRepository(IamDbContext context) : base(context) { }

      public override async Task<Role?> GetByIdAsync(Guid id)
      {
         return await _dbSet
            .Include(r => r.RoleFeatures)
            .FirstOrDefaultAsync(r => r.Id == id);
      }

      public void Update(Role role)
      {
         _dbSet.Update(role);
      }
   }
}
