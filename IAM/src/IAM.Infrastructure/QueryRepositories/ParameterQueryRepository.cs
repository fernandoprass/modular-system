using IAM.Domain.DTOs.Responses;
using IAM.Domain.Mappers;
using IAM.Domain.QueryRepositories;
using Microsoft.EntityFrameworkCore;

namespace IAM.Infrastructure.QueryRepositories
{
   public class ParameterQueryRepository(IamDbContext context) : IParameterQueryRepository
   {
      private readonly IamDbContext _context = context;

      public async Task<ParameterDto?> GetByIdAsync(Guid id)
      {
         var parameter = await _context.Parameters.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
         return parameter?.ToParameterDto();
      }

      public async Task<IEnumerable<ParameterLiteDto>> GetAllAsync()
      {
         return await _context.Parameters
            .AsNoTracking()
            .Select(p => p.ToParameterLiteDto())
            .ToListAsync();
      }

      public async Task<IEnumerable<ParameterLiteDto>> GetByGroupAsync(string group)
      {
         return await _context.Parameters
            .AsNoTracking()
            .Where(p => p.Group == group)
            .Select(p => p.ToParameterLiteDto())
            .ToListAsync();
      }

      public async Task<ParameterDto?> GetByGroupAndKeyAsync(string group, string key)
      {
         var parameter = await _context.Parameters
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Group == group && p.Key == key);
         return parameter?.ToParameterDto();
      }

      public async Task<string?> GetValueAsync(string @group, string key, Guid customerId)
      {
         var query = from p in _context.Parameters
                     join pc in _context.ParameterCustomers on new { ParameterId = p.Id, CustomerId = customerId } equals new { pc.ParameterId, pc.CustomerId } into pcGroup
                     from pc in pcGroup.DefaultIfEmpty()
                     where p.Group == @group && p.Key == key
                     select pc != null ? pc.Value : p.Value;

         return await query.FirstOrDefaultAsync();
      }
   }
}
