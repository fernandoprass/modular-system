using Shared.Domain.DTOs.Requests;
using Shared.Domain.DTOs.Responses;
using Myce.Response;

namespace Shared.Application.Contracts
{
   public interface IParameterService
   {
      //Method to be used in the UI for management of parameters, not intended for use in code
      //to get parameter values, for that use the GetByKeyAsync and GetValueAsync methods
      Task<Result<ParameterDto>> GetByIdAsync(Guid id);
      Task<Result<IEnumerable<ParameterLiteDto>>> GetAsync(ParameterSearchRequest request);
      Task<Result<ParameterValueDto>> GetValueAsync(string key);
      Task<Result> SaveOverrideValueAsync(Guid parameterId, ParameterOwnerUpdateRequest request);
      Task<Result> DeleteOverrideValueAsync(Guid parameterId);

      //Convenience methods to get parameter values directly by key, will throw an exception if the
      //parameter is not found or if the value cannot be converted to the expected type
      Task<Result<ParameterDto>> CreateAsync(ParameterCreateRequest request);
      Task<Result> UpdateAsync(Guid id, ParameterUpdateRequest request);
      Task<Result> DeleteAsync(Guid id);
      Task<ParameterDto?> GetByKeyAsync(string key);
      Task<bool> GetBoolAsync(string key);
      Task<int> GetIntAsync(string key);
      Task<decimal> GetDecimalAsync(string key);
      Task<DateTime> GetDateTimeAsync(string key);
      Task<string> GetStringAsync(string key);
   }
}
