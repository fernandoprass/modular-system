using IAM.Domain.Entities;
using IAM.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.Repositories
{
   public class ParameterRepository(IamDbContext context) : BaseRepository<Parameter>(context), IParameterRepository
   {
      public async Task<Parameter?> GetByKeyAsync(string key)
      {
         return await _dbSet.FirstOrDefaultAsync(p => p.Key == key);
      }
   }
}
