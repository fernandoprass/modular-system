using IAM.Domain.DTOs.Requests;
using IAM.Domain.DTOs.Responses;

using Myce.Response;

namespace IAM.Application.Contracts;

public interface ICustomerService
{
   Task<CustomerDto?> GetByIdAsync(Guid id);
   string GetRandomCode();
   Task<CustomerDto?> GetByNameAsync(string name);
   Task<Result> UpdateAsync(Guid id, CustomerUpdateRequest request);
   Task<Result> DeleteAsync(Guid id);
   Task<bool> ExistsAsync(Guid id);
   Task<Result> ValidateCreateCustomerAsync(CustomerCreateRequest request);
}