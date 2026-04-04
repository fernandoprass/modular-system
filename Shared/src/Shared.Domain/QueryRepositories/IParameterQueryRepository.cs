using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;

namespace Shared.Domain.QueryRepositories
{
   public interface IParameterQueryRepository
   {
      Task<ParameterDto?> GetByIdAsync(Guid id);
      Task<IEnumerable<ParameterLiteDto>> GetAllAsync(ParameterSearchRequest request);
      Task<ParameterDto?> GetByModuleGroupAndKeyAsync(string module, string group, string name);
      Task<string?> GetValueAsync(string key, Guid ownerId);
   }
}
