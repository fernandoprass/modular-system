using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;

namespace IAM.Domain.QueryRepositories
{
   public interface IParameterQueryRepository
   {
      Task<ParameterDto?> GetByIdAsync(Guid id);
      Task<IEnumerable<ParameterLiteDto>> GetAllAsync(ParameterSearchRequest request);
      Task<ParameterDto?> GetByModuleGroupAndKeyAsync(string key);
      Task<string?> GetValueAsync(string key, Guid customerId);
   }
}
