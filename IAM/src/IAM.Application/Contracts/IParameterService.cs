using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;
using Myce.Response;

namespace IAM.Application.Contracts
{
   public interface IParameterService
   {
      Task<ParameterDto?> GetByIdAsync(Guid id);
      Task<IEnumerable<ParameterLiteDto>> GetAsync(ParameterSearchRequest request);
      Task<ParameterDto?> GetByKeyAsync(string key);
      
      Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request);
      Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request);
      Task<Result> DeleteAsync(Guid id);

      Task<Result> SaveCustomerAsync(Guid id, ParameterCustomerUpdateRequest request);
      Task<Result> DeleteCustomerValueAsync(Guid parameterId, Guid customerId);

      Task<bool> GetBoolAsync(string key);
      Task<int> GetIntAsync(string key);
      Task<decimal> GetDecimalAsync(string key);
      Task<DateTime> GetDateTimeAsync(string key);
      Task<string> GetStringAsync(string key);
   }
}
