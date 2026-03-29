using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories
{
   public class ParameterRepository(IamDbContext context) : BaseRepository<Parameter>(context), IParameterRepository
   {
      public async Task<Parameter?> GetByGroupAndKeyAsync(string group, string key)
      {
         return await _dbSet.FirstOrDefaultAsync(p => p.Group == group && p.Key == key);
      }
   }
}
