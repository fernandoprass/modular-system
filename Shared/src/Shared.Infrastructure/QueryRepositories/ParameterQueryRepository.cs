using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Shared.Domain.Mappers;
using Shared.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace Shared.Infrastructure.QueryRepositories
{
   public class ParameterQueryRepository(SharedDbContext dbContext) : IParameterQueryRepository
   {
      private readonly SharedDbContext _dbContext = dbContext;

      public async Task<ParameterDto?> GetByIdAsync(Guid id)
      {
         var parameter = await _dbContext.Parameters
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);
         return parameter?.ToParameterDto();
      }

      public async Task<IEnumerable<ParameterLiteDto>> GetAllAsync(ParameterSearchRequest request)
      {
         var query = _dbContext.Parameters.AsNoTracking();

         if (!string.IsNullOrWhiteSpace(request.Module))
            query = query.Where(p => p.Module == request.Module);

         if (!string.IsNullOrWhiteSpace(request.Group))
            query = query.Where(p => p.Group == request.Group);

         if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(p => p.Name == request.Name);

         if (!string.IsNullOrWhiteSpace(request.Key))
            query = query.Where(p => p.Key == request.Key);

         if (!string.IsNullOrWhiteSpace(request.Title))
            query = query.Where(p => p.Title.Contains(request.Title));

         if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(p => p.Description.Contains(request.Description));

         return await query.Select(p => p.ToParameterLiteDto()).ToListAsync();
      }

      public async Task<ParameterDto?> GetByModuleGroupAndKeyAsync(string module, string group, string name)
      {
         var parameter = await _dbContext.Parameters
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Module == module && p.Group == group && p.Name == name);
         return parameter?.ToParameterDto();
      }

      public async Task<string?> GetValueAsync(string key, Guid ownerId)
      {
         var query = from p in _dbContext.Parameters
                     join pc in _dbContext.ParameterOverrides on new { ParameterId = p.Id, OwnerId = ownerId } equals new { pc.ParameterId, pc.OwnerId } into pcGroup
                     from pc in pcGroup.DefaultIfEmpty()
                     where p.Key == key
                     select pc != null ? pc.Value : p.Value;

         return await query.AsNoTracking()
            .SingleOrDefaultAsync();
      }
   }
}
