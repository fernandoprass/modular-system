using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IParameterService
   {
      Task<ParameterDto?> GetByIdAsync(Guid id);
      Task<IEnumerable<ParameterLiteDto>> GetAllAsync();
      Task<IEnumerable<ParameterLiteDto>> GetByGroupAsync(string group);
      Task<ParameterDto?> GetByGroupAndKeyAsync(string group, string key);
      
      Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request);
      Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request);
      Task<Result> DeleteAsync(Guid id);

      Task<Result> SaveCustomerOverrideAsync(string group, string key, ParameterCustomerUpdateRequest request);
      Task<Result> DeleteCustomerOverrideAsync(string group, string key);

      Task<bool> GetBoolAsync(string group, string key);
      Task<int> GetIntAsync(string group, string key);
      Task<decimal> GetDecimalAsync(string group, string key);
      Task<DateTime> GetDateTimeAsync(string group, string key);
      Task<string> GetStringAsync(string group, string key);
   }
}
