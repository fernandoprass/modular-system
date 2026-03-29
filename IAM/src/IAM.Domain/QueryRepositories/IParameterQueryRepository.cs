using IAM.Domain.DTOs.Responses;

namespace IAM.Domain.QueryRepositories
{
   public interface IParameterQueryRepository
   {
      Task<ParameterDto?> GetByIdAsync(Guid id);
      Task<IEnumerable<ParameterLiteDto>> GetAllAsync();
      Task<IEnumerable<ParameterLiteDto>> GetByGroupAsync(string group);
      Task<ParameterDto?> GetByGroupAndKeyAsync(string group, string key);
      Task<string?> GetValueAsync(string group, string key, Guid customerId);
   }
}
