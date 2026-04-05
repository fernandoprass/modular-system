using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities;
using Shared.Domain.Interfaces;

namespace Shared.Infrastructure.Repositories;

public class ParameterRepository(SharedDbContext dbContext) : BaseRepository<Parameter>(dbContext), IParameterRepository
{
   public async Task<Parameter?> GetByKeyAsync(string key)
   {
      return await _dbSet.FirstOrDefaultAsync(p => p.Key == key);
   }
}
