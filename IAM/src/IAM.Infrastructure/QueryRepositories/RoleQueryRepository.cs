using IAM.Domain.Entities;
using IAM.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.QueryRepositories
{
   public class RoleQueryRepository : IRoleQueryRepository
   {
      private readonly IamDbContext _context;

      public RoleQueryRepository(IamDbContext context)
      {
         _context = context;
      }

      public async Task<Role?> GetByIdAsync(Guid id)
      {
         return await _context.Roles
            .AsNoTracking()
            .Include(r => r.RoleFeatures)
               .ThenInclude(rf => rf.Feature)
            .FirstOrDefaultAsync(r => r.Id == id);
      }

      public async Task<IEnumerable<Role>> GetAllAsync(Guid customerId)
      {
         return await _context.Roles
            .AsNoTracking()
            .Where(r => r.CustomerId == null || r.CustomerId == customerId)
            .Include(r => r.RoleFeatures)
               .ThenInclude(rf => rf.Feature)
            .ToListAsync();
      }

      public async Task<bool> NameExistsAsync(string name, Guid? customerId)
      {
         return await _context.Roles
            .AnyAsync(r => r.Name == name && r.CustomerId == customerId);
      }

      public async Task<IEnumerable<Feature>> GetUserFeaturesAsync(Guid userId)
      {
          return await _context.UserRoles
              .AsNoTracking()
              .Where(ur => ur.UserId == userId)
              .SelectMany(ur => ur.Role.RoleFeatures.Select(rf => rf.Feature))
              .Distinct()
              .ToListAsync();
      }
   }
}
