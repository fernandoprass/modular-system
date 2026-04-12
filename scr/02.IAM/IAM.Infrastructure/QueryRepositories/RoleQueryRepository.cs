using IAM.Domain.Entities;
using IAM.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.QueryRepositories;

public class RoleQueryRepository(IamDbContext context) : IRoleQueryRepository
{
   private readonly IamDbContext _context = context;

   public async Task<Role?> GetByIdAsync(Guid id)
   {
      return await _context.Roles
         .AsNoTracking()
         .Include(r => r.RolePermissions)
            .ThenInclude(rf => rf.Permission)
         .FirstOrDefaultAsync(r => r.Id == id);
   }

   public async Task<IEnumerable<Role>> GetAllAsync(Guid customerId)
   {
      return await _context.Roles
         .AsNoTracking()
         .Where(r => r.CustomerId == null || r.CustomerId == customerId)
         .Include(r => r.RolePermissions)
            .ThenInclude(rf => rf.Permission)
         .ToListAsync();
   }

   public async Task<bool> NameExistsAsync(string name, Guid? customerId)
   {
      return await _context.Roles
         .AnyAsync(r => r.Name == name && r.CustomerId == customerId);
   }

   public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(Guid userId)
   {
       return await _context.UserRoles
           .AsNoTracking()
           .Where(ur => ur.UserId == userId)
           .SelectMany(ur => ur.Role.RolePermissions.Select(rf => rf.Permission))
           .Distinct()
           .ToListAsync();
   }
}
