using Microsoft.EntityFrameworkCore;
using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Shared.Domain.Enums;
using Shared.Domain.Interfaces;
using Shared.Domain.Mappers;

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

      public async Task<IEnumerable<ParameterLiteDto>> GetAllAsync(ParameterSearchRequestInternal request)
      {
         var query = from p in _dbContext.Parameters.AsNoTracking()
                     join o in _dbContext.ParameterOverrides on new
                     {
                        ParamId = p.Id,
                        Owner = (p.OverrideType == ParameterOverrideType.UserId ? request.UserId : request.UserOwnerId)
                     }
                     equals new
                     {
                        ParamId = o.ParameterId,
                        Owner = (Guid?)o.OwnerId
                     } into overrides
                     from subOver in overrides.DefaultIfEmpty()
                     select new { p, subOver };

         if (!string.IsNullOrWhiteSpace(request.Module))
            query = query.Where(x => x.p.Module == request.Module);

         if (!string.IsNullOrWhiteSpace(request.Group))
            query = query.Where(x => x.p.Group == request.Group);

         if (!string.IsNullOrWhiteSpace(request.Name))
            query = query.Where(x => x.p.Name == request.Name);

         if (!string.IsNullOrWhiteSpace(request.Key))
            query = query.Where(x => x.p.Key == request.Key);

         if (!string.IsNullOrWhiteSpace(request.Description))
            query = query.Where(x => x.p.Description.Contains(request.Description));

         if (!request.IsSystemAdmin)
            query = query.Where(x => x.p.IsVisible);

         return await query.Select(x => new ParameterLiteDto
         {
            Id = x.p.Id,
            Key = x.p.Key,
            Title = x.p.Title,
            Description = x.p.Description,
            Type = x.p.Type,
            Value = x.subOver != null ? x.subOver.Value : x.p.Value,
            OverrideType = x.subOver != null
         }).ToListAsync();
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
